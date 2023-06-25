param (
    [Parameter(Mandatory = $true)]
    [string]$libFuzzer,
    [Parameter(Mandatory = $true)]
    [string]$project,
    [Parameter(Mandatory = $true)]
    [string]$corpus,
    [string]$dict = $null,
    [int]$timeout = 10,
    [string]$command = "sharpfuzz"
)

Set-StrictMode -Version Latest

$outputDir = "bin"

if (Test-Path $outputDir) {
    Remove-Item -Recurse -Force $outputDir
}

dotnet publish $project -c release -o $outputDir --self-contained

$projectName = (Get-Item $project).BaseName
$project = Join-Path $outputDir $projectName
$target = Join-Path $outputDir System.Text.Json.dll

& $command $target

if ($LastExitCode -ne 0) {
    Write-Error "An error occurred while instrumenting $fuzzingTarget"
    exit 1
}

& $libFuzzer -timeout="$timeout" --target_path=$project $corpus
