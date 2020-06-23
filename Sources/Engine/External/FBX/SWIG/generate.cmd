del "..\FbxWrapperSln\FbxWrapper\*.cs" /f
swigwin-3.0.12_original\swig -c++ -csharp -namespace Fbx -I"FBXSDK_ChangedHeaders" -outdir "..\FbxWrapperSln\FbxWrapper" -o "..\FbxWrapperSln\FbxWrapperNative\FbxWrapperNative.cpp" fbxwapper.i
pause