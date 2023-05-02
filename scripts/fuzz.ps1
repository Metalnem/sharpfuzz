param (
    [Parameter(Mandatory = $true)]
    [string]$projectPath,
    [Parameter(Mandatory = $true)]
    [string[]]$fuzzingTargets,
    [string]$testcasesDir = "testcases",
    [string]$findingsDir = "findings"
)

Set-StrictMode -Version Latest

$outputDir = "bin"

if (Test-Path $outputDir) { Remove-Item -Recurse -Force $outputDir }
if (Test-Path $findingsDir) { Remove-Item -Recurse -Force $findingsDir }

dotnet publish $projectPath -c release -o $outputDir

foreach ($fuzzingTarget in $fuzzingTargets) {
    $fuzzingTargetPath = Join-Path $outputDir $fuzzingTarget
    Write-Output "Instrumenting $($fuzzingTargetPath)"
    sharpfuzz $fuzzingTargetPath
    
    if ($LastExitCode -ne 0) {
        Write-Error "An error occurred while instrumenting $($fuzzingTargetPath)"
        exit 1
    }
}

$projectName = (Get-Item $projectPath).BaseName
$projectPath = Join-Path $outputDir "$projectName.dll"

afl-fuzz -i $testcasesDir -o $findingsDir -t 10000 -m 10000 dotnet $projectPath
