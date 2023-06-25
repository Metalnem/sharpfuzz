param (
    [Parameter(Mandatory = $true)]
    [string]$project,
    [Parameter(Mandatory = $true)]
    [string]$i,
    [string]$x = $null,
    [int]$t = 10000,
    [string]$command = "sharpfuzz"
)

Set-StrictMode -Version Latest

$outputDir = "bin"
$findingsDir = "findings"

if (Test-Path $outputDir) { 
    Remove-Item -Recurse -Force $outputDir 
}

if (Test-Path $findingsDir) {
    Remove-Item -Recurse -Force $findingsDir 
}

dotnet publish $project -c release -o $outputDir --self-contained

Write-Output "Instrumenting System.Text.Json.dll"

& $command  "$outputDir"/System.Text.Json.dll

if ($LastExitCode -ne 0) {
    Write-Error "An error occurred while instrumenting $fuzzingTarget"
    exit 1
}

$env:AFL_SKIP_BIN_CHECK = 1

afl-fuzz -i $i -o $findingsDir -t $t -m none bin/SystemTextJson
