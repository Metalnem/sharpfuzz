FROM ubuntu:trusty
WORKDIR /app

RUN apt-get update \
	&& apt-get install -y wget \
	&& wget -qO - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - \
	&& echo 'deb http://apt.llvm.org/trusty/ llvm-toolchain-trusty main' >> /etc/apt/sources.list.d/llvm.list \
	&& echo 'deb-src http://apt.llvm.org/trusty/ llvm-toolchain-trusty main' >> /etc/apt/sources.list.d/llvm.list \
	&& echo 'deb http://ppa.launchpad.net/ubuntu-toolchain-r/test/ubuntu trusty main' >> /etc/apt/sources.list.d/llvm.list \
	&& gpg --keyserver keyserver.ubuntu.com --recv 1E9377A2BA9EF27F \
	&& gpg --export --armor 1E9377A2BA9EF27F | sudo apt-key add -

RUN apt-get update && apt-get install -y \
	clang-9 \
	libfuzzer-9-dev \
	llvm-9 \
	llvm-9-dev

COPY libfuzzer-dotnet.cc /app
RUN clang++-9 -fsanitize=fuzzer libfuzzer-dotnet.cc -o libfuzzer-dotnet
