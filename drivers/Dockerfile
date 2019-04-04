FROM debian:8
WORKDIR /app
COPY build.sh libFuzzer.c libFuzzer.list /app/
RUN /bin/sh build.sh
