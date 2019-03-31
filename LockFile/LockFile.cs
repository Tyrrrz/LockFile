using System;
using System.IO;
using System.Threading;
using LockFile.Internal;

namespace LockFile
{
    /// <summary>
    /// Represents an exclusive resource backed by a file.
    /// </summary>
    public partial class LockFile : IDisposable
    {
        /// <summary>
        /// File stream that represents this lock file.
        /// </summary>
        public FileStream FileStream { get; }

        /// <summary>
        /// Initializes an instance of <see cref="LockFile"/>.
        /// </summary>
        public LockFile(FileStream fileStream)
        {
            FileStream = fileStream.GuardNotNull(nameof(fileStream));
        }

        /// <inheritdoc />
        public void Dispose() => FileStream.Dispose();
    }

    public partial class LockFile
    {
        /// <summary>
        /// Tries to acquire a lock file with given file path.
        /// Returns null if the file is already in use.
        /// </summary>
        public static LockFile TryAcquire(string filePath)
        {
            filePath.GuardNotNull(nameof(filePath));

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

        /// <summary>
        /// Repeatedly tries to acquire a lock file, until the operation succeeds or is canceled.
        /// </summary>
        public static LockFile WaitAcquire(string filePath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            filePath.GuardNotNull(nameof(filePath));

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