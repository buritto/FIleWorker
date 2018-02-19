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
            return command.Execute(Path);
        }
    }


}
