# BenchmarkDotNet on Android

This has a number of hacks to get BenchmarkDotNet running in a .NET 6
Android application. Some of these we can contribute to
BenchmarkDotNet for future releases.

Note that you can't simply run the app to run the benchmarks, I've
setup up an Android instrumentation class and custom MSBuild target:

    ./bin/dotnet/dotnet build src/Core/tests/Benchmarks.Droid/Benchmarks.Droid.csproj -t:Benchmark -c Release

Use `Release` builds when recording your final timing on a physical device.

This will print output like:

    I/DOTNET  : // * Summary *
    I/DOTNET  :
    I/DOTNET  : BenchmarkDotNet=v0.13.1, OS=Unknown
    I/DOTNET  : Unknown processor
    I/DOTNET  :   [Host] : .NET 6.0.0 (6.0.21.52210), Arm64 RyuJIT
    I/DOTNET  :
    I/DOTNET  : Toolchain=InProcessEmitToolchain
    I/DOTNET  :
    I/DOTNET  : |            Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
    I/DOTNET  : |------------------ |-----------:|---------:|---------:|-------:|----------:|
    I/DOTNET  : |       ContentView |   219.2 µs |  0.81 µs |  0.76 µs | 1.4648 |      6 KB |
    I/DOTNET  : |            Border |   223.4 µs |  3.41 µs |  3.19 µs | 1.2207 |      5 KB |
    I/DOTNET  : | ActivityIndicator |   358.0 µs |  2.25 µs |  2.10 µs | 0.9766 |      5 KB |
    I/DOTNET  : |             Label |   541.1 µs |  2.38 µs |  2.23 µs | 0.9766 |      5 KB |
    I/DOTNET  : |             Entry | 1,792.9 µs | 21.60 µs | 20.21 µs |      - |     10 KB |
    I/DOTNET  :
    I/DOTNET  : // * Legends *
    I/DOTNET  :   Mean      : Arithmetic mean of all measurements
    I/DOTNET  :   Error     : Half of 99.9% confidence interval
    I/DOTNET  :   StdDev    : Standard deviation of all measurements
    I/DOTNET  :   Gen 0     : GC Generation 0 collects per 1000 operations
    I/DOTNET  :   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
    I/DOTNET  :   1 µs      : 1 Microsecond (0.000001 sec)
    I/DOTNET  :
    I/DOTNET  : // * Diagnostic Output - MemoryDiagnoser *
    I/DOTNET  :
    I/DOTNET  :
    I/DOTNET  : // ***** BenchmarkRunner: End *****
    I/DOTNET  : // ** Remained 0 benchmark(s) to run **
    I/DOTNET  : Run time: 00:01:50 (110.92 sec), executed benchmarks: 5
    I/DOTNET  :
    I/DOTNET  : Global total time: 00:01:51 (111.12 sec), executed benchmarks: 5
    I/DOTNET  : // * Artifacts cleanup *
    D/MAUI    : Benchmark complete, success: True
