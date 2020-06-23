REM https://github.com/AndresTraks/BulletSharpPInvoke/wiki/Build-Instructions

mkdir build
cd build
cmake -DBUILD_BULLET3=ON -DUSE_DOUBLE_PRECISION=ON -G "Visual Studio 15 2017 Win64" ..
start .
