#!/bin/sh
set -eux

docker build -t libfuzzer-dotnet .
docker container create --name extract-libfuzzer-dotnet libfuzzer-dotnet
docker container cp extract-libfuzzer-dotnet:/app/libfuzzer-dotnet .
docker container cp extract-libfuzzer-dotnet:/app/libprotobuf-mutator/build/src/libfuzzer-dotnet/libfuzzer-proto-dotnet .
docker container rm -f extract-libfuzzer-dotnet
zip -m libfuzzer-dotnet.zip libfuzzer-dotnet libfuzzer-proto-dotnet
