using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FileWorkerApp
{
    internal class Pairs
    {
        public string Path { get; }
        public byte[] Hash { get; }

        public Pairs(string path, byte[] hash)
        {
            Path = path;
            Hash = hash;
        }
    }

    public class Md5Executor : IExecutable
    {
        private readonly string defoultPathForSaveHash = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "MD5HashesLog.txt";
        private readonly List<Pairs> hashPool = new List<Pairs>();
        private List<Pairs> hashForFolderPool = new List<Pairs>();
        private int nesting;
        private int poolCountDefoult = 2048;


        private bool ExecuteFolder(string path)
        {
            var fileInDirectory = Directory.EnumerateFileSystemEntries(path);
            nesting++;

            foreach (var filePath in fileInDirectory)
            {
                Execute(filePath);
            }

            var hashConcat = hashForFolderPool.Skip(hashForFolderPool.Count - fileInDirectory.Count())
                .Select(pair => pair.Hash)
                .Aggregate((hash, nextHash) => hash.Concat(nextHash).ToArray());
            var hashFromConcatHash = MD5.Create().ComputeHash(hashConcat);
            return SaveHashFolderAndClearPool(Convert.ToBase64String(hashFromConcatHash), path,
                fileInDirectory.Count());
        }

        public bool Execute(string path)
        {
            if (Directory.Exists(path))
            {
                return ExecuteFolder(path);
            }
            var successfulCompletion = true;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        stream.CopyTo(memoryStream);
                        byte[] hash = MD5.Create().ComputeHash(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                        if (nesting == 0)
                        {
                            if (hashPool.Count == poolCountDefoult)
                            {
                                SavePool();
                            }
                            hashPool.Add(new Pairs(path, hash));
                        }
                        else
                            hashForFolderPool.Add(new Pairs(path, hash));
                    }
                }
            }
            catch (Exception e)
            {
                successfulCompletion = false;
            }
            
            return successfulCompletion;
        }

        private void SavePool()
        {
            using (var sw = new StreamWriter(defoultPathForSaveHash, true))
            {
                hashPool.Select(pair => pair.Path + ":" + Convert.ToBase64String(pair.Hash)).ToList().ForEach(hash => sw.Write(hash));
            }
            hashPool.Clear();
        }

        private bool SaveHashFolderAndClearPool(string hashFolder,string pathFolder, int sizeFolderObject)
        {
            var successfulCompletion = true;
            try
            {
                using (var sw = new StreamWriter(defoultPathForSaveHash, true))
                {
                    sw.Write(pathFolder + " " + hashFolder);
                    hashForFolderPool.Skip(hashForFolderPool.Count - sizeFolderObject).Select(pair => pair.Path + ":" + Convert.ToBase64String(pair.Hash)).ToList().ForEach(hash =>
                        sw.WriteLine(hash));
                }
            }
            catch (Exception e)
            {
                successfulCompletion = false;
            }
            nesting--;
            hashForFolderPool = hashForFolderPool.Where(pair => pair.Path != pathFolder).ToList();
            return successfulCompletion;
        }
    }
}
