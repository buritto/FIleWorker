using System.IO;

namespace FileWorkerApp
{
    public interface IExecutable
    {
        bool Execute(FileStream file);
    }
}
