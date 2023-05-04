param (
    [Parameter(Mandatory = $true)]
    [string]$projectPath,
    [string]$testcasesDir = "testcases",
    [string]$findingsDir = "findings"
)

Set-StrictMode -Version Latest

if (!(Test-Path $testcasesDir)) {
    Write-Error "Testcases directory $testcasesDir does not exist"
    exit 1
}

$testcases = Get-ChildItem $testcasesDir

if (!$testcases) {
    Write-Error "Testcases directory $testcasesDir is empty"
    exit 1
}

$outputDir = "bin"

if (Test-Path $outputDir) { Remove-Item -Recurse -Force $outputDir }
if (Test-Path $findingsDir) { Remove-Item -Recurse -Force $findingsDir }

dotnet publish $projectPath -c release -o $outputDir

$projectName = (Get-Item $projectPath).BaseName
$projectDll = "$projectName.dll"
$projectPath = Join-Path $outputDir $projectDll

$exclusions = @(
    "dnlib.dll",
    "SharpFuzz.dll",
    "SharpFuzz.Common.dll",
    $projectDll
)

$fuzzingTargets = Get-ChildItem $outputDir -Filter *.dll `
| Where-Object { $_.Name -notin $exclusions } `
| Where-Object { $_.Name -notlike "System.*.dll" }

foreach ($fuzzingTarget in $fuzzingTargets) {
    Write-Output "Instrumenting $fuzzingTarget"
    sharpfuzz $fuzzingTarget
    
    if ($LastExitCode -ne 0) {
        Write-Error "An error occurred while instrumenting $fuzzingTarget"
        exit 1
    }
}

afl-fuzz -i $testcasesDir -o $findingsDir -t 10000 -m 10000 dotnet $projectPath
