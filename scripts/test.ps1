New-Item -Path "corpus/test" -ItemType File -Force -Value "W"
& scripts/fuzz.ps1 tests/Library.Fuzz/Library.Fuzz.csproj -i corpus

$output = Get-Content -Path "./findings/.cur_input" -Raw
$crasher = "Whoopsie"

if (-not $output.Contains($crasher)) {
    Write-Error "Crasher is missing from the AFL output"
    exit 1
}

Write-Host $crasher
exit 0
