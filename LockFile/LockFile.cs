using System;
using System.IO;
using System.Threading;

namespace LockFile
{
    public partial class LockFile : IDisposable
    {
        public FileStream FileStream { get; }

        public LockFile(FileStream fileStream)
        {
            FileStream = fileStream;
        }

        public void Dispose() => FileStream.Dispose();
    }

    public partial class LockFile
    {
        public static LockFile TryAcquire(string filePath)
        {
            try
            {
                var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                return new LockFile(fileStream);
            }
            // When access to file is denied, an IOException (not derived) is thrown
            catch (IOException ex) when (ex.GetType() == typeof(IOException))
            {
                return null;
            }
        }

        public static LockFile WaitAcquire(string filePath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (true)
            {
                // Throw if canceled
                cancellationToken.ThrowIfCancellationRequested();

                // Try to acquire lock file
                var lockFile = TryAcquire(filePath);
                if (lockFile != null)
                    return lockFile;
            }
        }
    }
}