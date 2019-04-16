FROM debian:9
WORKDIR /app

RUN apt-get update \
	&& apt-get install -y git gnupg wget \
	&& wget -qO - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - \
	&& echo 'deb http://apt.llvm.org/stretch/ llvm-toolchain-stretch main' >> /etc/apt/sources.list.d/llvm.list \
	&& echo 'deb-src http://apt.llvm.org/stretch/ llvm-toolchain-stretch main' >> /etc/apt/sources.list.d/llvm.list

RUN apt-get update && apt-get install -y \
	clang-9\
	libfuzzer-9-dev\
	llvm-9 \
	llvm-9-dev

COPY libfuzzer-dotnet.c /app
RUN clang-9 -fsanitize=fuzzer libfuzzer-dotnet.c -o libfuzzer-dotnet
