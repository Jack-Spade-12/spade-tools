
namespace com.sc.project.spadetools {
    
    class ExtractFileNames {

        // Primary Variables
        private List<string> Files { get; set; }
        private List<string> FileTypes { get; set; }
        private List<String> FailedDirectories { get; set; }
        private File rootDirectory { get; set; }


        // Supporting Variables
        private bool isFullPath;
        private bool isGroupByFileTypes;
        private bool isSaveOutput;

        public ExtractFileNames(string[] arguments)
        {

        }

        protected virtual SetArguments(string[] arguments)
        {
            
        }

    }

}

