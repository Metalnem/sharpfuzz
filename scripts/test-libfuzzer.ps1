$libFuzzer = "libfuzzer-dotnet-ubuntu"
$uri = "https://github.com/metalnem/libfuzzer-dotnet/releases/latest/download/$libFuzzer"
$corpus = "corpus"

Invoke-WebRequest -Uri $uri -OutFile $libFuzzer
New-Item -Path $corpus -ItemType Directory

dotnet publish src/SharpFuzz.CommandLine/SharpFuzz.CommandLine.csproj `
    --output out `
    --configuration release `
    --framework net8.0

& scripts/fuzz-libfuzzer.ps1 `
    -libFuzzer "./$libFuzzer" `
    -project tests/Library.LibFuzzer/Library.LibFuzzer.csproj `
    -corpus $corpus `
    -command out/SharpFuzz.CommandLine

$crasher = "Whoopsie"

Write-Host $crasher
exit 0
