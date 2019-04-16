FROM debian:9
WORKDIR /app

RUN apt-get update \
	&& apt-get install -y git gnupg wget \
	&& wget -qO - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - \
	&& echo 'deb http://apt.llvm.org/stretch/ llvm-toolchain-stretch main' >> /etc/apt/sources.list.d/libFuzzer.list \
	&& echo 'deb-src http://apt.llvm.org/stretch/ llvm-toolchain-stretch main' >> /etc/apt/sources.list.d/libFuzzer.list \
	&& git clone https://github.com/google/libprotobuf-mutator.git

RUN apt-get update && apt-get install -y \
	binutils \
	clang-9 \
	cmake \
	dh-autoreconf \
	libfuzzer-9-dev \
	liblzma-dev \
	libz-dev \
	llvm-9 \
	llvm-9-dev \
	ninja-build \
	pkg-config

WORKDIR libprotobuf-mutator
COPY CMakeLists.txt libfuzzer-http.proto libfuzzer-proto-dotnet.cc src/libfuzzer/
WORKDIR build

RUN cmake .. -GNinja \
	-DCMAKE_BUILD_TYPE=Debug \
	-DCMAKE_C_COMPILER=clang-9 \
	-DCMAKE_CXX_COMPILER=clang++-9 \
	-DLIB_PROTO_MUTATOR_DOWNLOAD_PROTOBUF=ON

RUN ninja
