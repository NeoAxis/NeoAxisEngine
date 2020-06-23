# BulletSharp
BulletSharp is a .NET wrapper for the [Bullet](http://bulletphysics.org/) physics library.

This version uses Platform Invoke. There is also an equivalent version written in C++/CLI: https://github.com/AndresTraks/BulletSharp

libbulletc is a C interface to Bullet. It compiles into a .dll or .so file that exports Bullet functions.

BulletSharpPInvoke is a .NET library that proxies calls from .NET to libbulletc.

The benefit of P/Invoke over C++/CLI is that it runs on all platforms that support P/Invoke into shared user-mode libraries (Windows, Unix, Mac OS). See also [Supported platforms](https://github.com/AndresTraks/BulletSharp/wiki/Supported-platforms).