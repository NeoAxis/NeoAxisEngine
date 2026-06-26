:: Install to C:\emsdk. Verify path: C:\emsdk\emsdk\python\3.13.3_64bit\python.exe

:: To compile .NET 10 projects, use Emscripten 3.1.56 exactly.
:: C:\emsdk\emsdk\emsdk.bat install 3.1.56
:: C:\emsdk\emsdk\emsdk.bat activate 3.1.56

:: PATH environment variables are not needed.

Project\Binaries\NeoAxis.Internal\Platforms\Windows\CommandLineTools\CommandLineTools.exe -compile "..\..\..\..\..\..\Sources\Engine\NeoAxis.Core.Native\NeoAxisCoreNative.Web.Debug.compile"
