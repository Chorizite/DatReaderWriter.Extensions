using DatReaderWriter.Lib.IO.DatBTree;
using DatReaderWriter.Options;
using DatReaderWriter.DBObjs;


namespace DatReaderWriter.Extensions;

public static class DatDatabaseExtensions {
    /// <summary>
    /// Defragments the dat database by rewriting it to a new file with sequential blocks.
    /// </summary>
    /// <param name="db">The source database</param>
    /// <param name="outputPath">The path to write the new defragmented dat file to</param>
    /// <param name="progress">Optional progress callback (0.0 to 1.0)</param>
    /// <returns>The number of bytes freed.</returns>
    public static int Defragment(this DatDatabase db, string outputPath, Action<float>? progress = null) {
        if (string.IsNullOrEmpty(outputPath)) {
            throw new ArgumentNullException(nameof(outputPath));
        }

        // Dispose the new database when done
        using var newDb = new DatDatabase(opt => {
            opt.FilePath = outputPath;
            opt.AccessType = DatAccessType.ReadWrite;
            opt.FileCachingStrategy = FileCachingStrategy.Never;
            opt.IndexCachingStrategy = IndexCachingStrategy.Never;
        });

        // Initialize the new database with same parameters as source
        newDb.BlockAllocator.InitNew(
            db.Header.Type,
            db.Header.SubSet,
            db.Header.BlockSize,
            0 // Start with 0 free blocks to minimize file size
        );

        newDb.Header.MasterMapId = db.Header.MasterMapId;
        newDb.Header.OldLRU = db.Header.OldLRU;
        newDb.Header.NewLRU = db.Header.NewLRU;
        newDb.Header.UseLRU = db.Header.UseLRU;
        newDb.BlockAllocator.SetVersion(db.Header.Version, db.Header.EngineVersion, db.Header.GameVersion,
            db.Header.MajorVersion, db.Header.MinorVersion);

        // Get all files from source
        // We order by ID to ensure the tree is built nicely and blocks are sequential by ID (mostly)
        var files = db.Tree.OrderBy(f => f.Id).ToList();
        var totalFiles = files.Count;
        var processed = 0;

        foreach (var file in files) {
            if (db.TryGetFileBytes(file.Id, out var bytes)) {
                // Write block to new DB using the allocator directly to get the new offset
                var newOffset = newDb.BlockAllocator.WriteBlock(bytes, bytes.Length);

                // Create new file entry
                var newEntry = new DatBTreeFile {
                    Id = file.Id,
                    Flags = file.Flags,
                    Offset = newOffset,
                    Size = (uint)bytes.Length,
                    Date = file.Date,
                    Iteration = file.Iteration
                };

                // Insert into new tree
                newDb.Tree.Insert(newEntry);
            }

            processed++;
            progress?.Invoke((float)processed / totalFiles);
        }

        if (db.TryGet<Iteration>(0xFFFF0001, out var iteration)) {
            newDb.TryWriteFile(iteration);
        }

        return db.Header.FileSize - newDb.Header.FileSize;
    }
}
