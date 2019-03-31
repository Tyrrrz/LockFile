# LockFile

[![Build](https://img.shields.io/appveyor/ci/Tyrrrz/LockFile/master.svg)](https://ci.appveyor.com/project/Tyrrrz/LockFile)
[![Tests](https://img.shields.io/appveyor/tests/Tyrrrz/LockFile/master.svg)](https://ci.appveyor.com/project/Tyrrrz/LockFile)
[![NuGet](https://img.shields.io/nuget/v/LockFile.svg)](https://nuget.org/packages/LockFile)
[![NuGet](https://img.shields.io/nuget/dt/LockFile.svg)](https://nuget.org/packages/LockFile)

LockFile is the simplest lock file implementation for .NET.

## Download

- [NuGet](https://nuget.org/packages/LockFile): `dotnet add package LockFile`
- [Continuous integration](https://ci.appveyor.com/project/Tyrrrz/LockFile)

## Features

- Dead simple
- Targets .NET Framework 4.5+ and .NET Standard 2.0+

## Usage

Lock files are acquired by opening a file with `FileShare.None`, which guarantees exclusive access to file. Lock files are released when they are disposed.

### Try to acquire a lock file once

```c#
using (var lockFile = LockFile.TryAcquire("some.lock"))
{
    if (lockFile != null)
    {
        // Lock file acquired
    }
}
```

### Wait until lock file is released and acquire it

```c#
using (var lockFile = LockFile.WaitAcquire("some.lock"))
{
    // Lock file is eventually acquired
}
```

Or you can set a timeout via `CancellationToken`:

```c#
using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
using (var lockFile = LockFile.WaitAcquire("some.lock", cts.Token))
{
    // If lock file is not acquired within 2 seconds, an exception is thrown
}
```

## Libraries used

- [NUnit](https://github.com/nunit/nunit)