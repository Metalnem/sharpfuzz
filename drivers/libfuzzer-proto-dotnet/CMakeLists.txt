protobuf_generate_cpp(LIB_FUZZER_PROTO_SRCS
                      LIB_FUZZER_PROTO_HDRS
                      libfuzzer-http.proto)

add_library(libfuzzer-http-proto
            ${LIB_FUZZER_PROTO_SRCS})

set_property(TARGET libfuzzer-http-proto
             PROPERTY COMPILE_FLAGS ${NO_FUZZING_FLAGS})

add_executable(libfuzzer-proto-dotnet libfuzzer-proto-dotnet.cc)

target_link_libraries(libfuzzer-proto-dotnet
                      libfuzzer-http-proto
                      protobuf-mutator-libfuzzer
                      ${LIB_PROTO_MUTATOR_FUZZER_LIBRARIES})

set_property(TARGET libfuzzer-proto-dotnet
             PROPERTY COMPILE_FLAGS ${FUZZING_FLAGS})

set_property(TARGET libfuzzer-proto-dotnet
             PROPERTY LINK_FLAGS ${FUZZING_FLAGS_BINARY})
