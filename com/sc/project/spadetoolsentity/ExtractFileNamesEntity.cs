
namespace com.sc.project.spadetoolsentity
{
    protected class ExtractFileNamesEntity
    {
        private File rootDirectory;
        private string saveDirectory;
        private List<string> files;
        private List<string> fileTypes;
        private List<string> excludedFileTypes;
        private List<string> failedDirectories;
        private bool isFullPath;
        private bool isGroupByFileTypes;
        private bool isSaveOutput;
    }
}