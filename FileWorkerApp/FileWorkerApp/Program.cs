using System.IO;

namespace FileWorkerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var fw = new FileWorker(new Md5Executor());
            var path = "C:\\test";
            fw.IsRecursive = true;
            var fileInDirectory = Directory.EnumerateFileSystemEntries(path);
            foreach (var filePath in fileInDirectory)
            {
                if (Directory.Exists(filePath) && !fw.IsRecursive)
                    continue;
                fw.Path = filePath;
                fw.Execute();
            }

        }
    }
}
