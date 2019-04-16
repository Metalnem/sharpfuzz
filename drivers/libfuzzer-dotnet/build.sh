#!/bin/sh
set -eux

docker build -t libfuzzer-dotnet .
docker container create --name extract-libfuzzer-dotnet libfuzzer-dotnet
docker container cp extract-libfuzzer-dotnet:/app/libfuzzer-dotnet ./
docker container rm -f extract-libfuzzer-dotnet
