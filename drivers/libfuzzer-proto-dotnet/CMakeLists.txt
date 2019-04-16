add_library(protobuf-mutator-libfuzzer
            libfuzzer_mutator.cc
            libfuzzer_macro.cc)

target_link_libraries(protobuf-mutator-libfuzzer
                      protobuf-mutator
                      ${PROTOBUF_LIBRARIES})

set_target_properties(protobuf-mutator-libfuzzer PROPERTIES
                      COMPILE_FLAGS "${NO_FUZZING_FLAGS}"
                      SOVERSION 0)

install(TARGETS protobuf-mutator-libfuzzer
        ARCHIVE DESTINATION ${LIB_DIR}
        LIBRARY DESTINATION ${LIB_DIR})

protobuf_generate_cpp(LIB_FUZZER_PROTO_SRCS
                      LIB_FUZZER_PROTO_HDRS
                      libfuzzer-http.proto)

add_library(libfuzzer-proto
            ${LIB_FUZZER_PROTO_SRCS})

set_property(TARGET libfuzzer-proto
             PROPERTY COMPILE_FLAGS ${NO_FUZZING_FLAGS})

add_executable(libfuzzer-proto-dotnet libfuzzer-proto-dotnet.cc)

target_link_libraries(libfuzzer-proto-dotnet
                      libfuzzer-proto
                      protobuf-mutator-libfuzzer
                      ${LIB_PROTO_MUTATOR_FUZZER_LIBRARIES})

set_property(TARGET libfuzzer-proto-dotnet
             PROPERTY COMPILE_FLAGS ${FUZZING_FLAGS})

set_property(TARGET libfuzzer-proto-dotnet
             PROPERTY LINK_FLAGS ${FUZZING_FLAGS_BINARY})
