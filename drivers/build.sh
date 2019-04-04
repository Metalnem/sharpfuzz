#/bin/sh
set -eux

# Download the LLVM archive signature
apt-get update && apt-get install -y wget
wget -qO - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add -

# Install LLVM, clang, and libFuzzer
mv libFuzzer.list /etc/apt/sources.list.d/libFuzzer.list
apt-get update && apt-get install -y llvm-9 llvm-9-dev clang-9 libfuzzer-9-dev

# Build the libFuzzer .NET driver
clang-9 -fsanitize=fuzzer libFuzzer.c -o libfuzzer-dotnet
