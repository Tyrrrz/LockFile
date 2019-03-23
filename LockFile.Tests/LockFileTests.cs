using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LockFile.Tests
{
    [TestFixture]
    public class LockFileTests
    {
        public string TestDirPath => TestContext.CurrentContext.TestDirectory;

        public string TempDirPath => Path.Combine(TestDirPath, "Temp");

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(TempDirPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(TempDirPath))
                Directory.Delete(TempDirPath, true);
        }

        [Test]
        public void LockFileInfo_TryAcquire_Test()
        {
            var lockFilePath = Path.Combine(TempDirPath, Guid.NewGuid().ToString());

            using (var lockFile = LockFile.TryAcquire(lockFilePath))
            {
                Assert.That(lockFile, Is.Not.Null);
            }
        }

        [Test]
        public void LockFileInfo_TryAcquire_AlreadyAcquired_Test()
        {
            var lockFilePath = Path.Combine(TempDirPath, Guid.NewGuid().ToString());

            using (LockFile.TryAcquire(lockFilePath))
            {
                var lockFile = LockFile.TryAcquire(lockFilePath);
                Assert.That(lockFile, Is.Null);
            }
        }

        [Test]
        public void LockFileInfo_WaitAcquire_Test()
        {
            var lockFilePath = Path.Combine(TempDirPath, Guid.NewGuid().ToString());

            using (var originalLockFile = LockFile.TryAcquire(lockFilePath))
            {
                Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(_ => originalLockFile.Dispose());

                using (var newLockFile = LockFile.WaitAcquire(lockFilePath))
                {
                    Assert.That(newLockFile, Is.Not.Null);
                }
            }
        }

        [Test]
        public void LockFileInfo_WaitAcquire_Cancellation_Test()
        {
            var lockFilePath = Path.Combine(TempDirPath, Guid.NewGuid().ToString());

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0.5)))
            using (LockFile.TryAcquire(lockFilePath))
            {
                Assert.Throws<OperationCanceledException>(() =>
                    LockFile.WaitAcquire(lockFilePath, cts.Token));
            }
        }
    }
}