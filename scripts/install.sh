#/bin/sh
set -eux

# Download and extract the latest afl-fuzz source package
wget https://github.com/AFLplusplus/AFLplusplus/archive/refs/tags/v4.30c.tar.gz
tar -xvzf v4.30c.tar.gz

rm v4.30c.tar.gz
cd AFLplusplus-4.30c/

# Install afl-fuzz
sudo make install
cd ..
rm -rf AFLplusplus-4.30c/

# Install SharpFuzz.CommandLine global .NET tool
dotnet tool install --global SharpFuzz.CommandLine
