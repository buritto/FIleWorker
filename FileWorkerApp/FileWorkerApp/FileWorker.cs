using System.IO;

namespace FileWorkerApp
{
    public class FileWorker
    {
        private IExecutable command;
        public bool IsRecursive { get; set; }
        public string Path { get; set; }

        public FileWorker(IExecutable command)
        {
            this.command = command;
        }

        public bool Execute()
        {
            var stream = new FileStream(Path, FileMode.Open);
            return command.Execute(stream);
        }
    }


}
