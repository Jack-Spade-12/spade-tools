
namespace com.sc.project.spadetools 
{
    
    class ExtractFileNames : ExtractFileNamesEntity
    {

        // Properties
        protected string RootDirectory
        {
            get
            {
                return rootDirectory.GetFullPath();
            }
            
            set
            {
                // todo : create IsValidFile(string filepath) function
                if (IsValidFile(value))
                {
                    rootDirectory = value;
                }
                else
                {
                    // todo : add throw error
                }
            }
        }

        protected string SaveDirectory {
            get 
            {
                return saveDirectory;
            }
             
            set
            {
                // todo : create IsValidFile(string filepath) function
                if (IsValidFile(value))
                {
                    saveDirectory = value;
                }
                else
                {
                    saveDirectory = rootDirectory;
                }
            }
        }
        
        public ExtractFileNames(string[] arguments)
        {
            rootDirectory = "";
            saveDirectory = "";
            files = new List<string>();
            fileTypes = new List<string>();
            excludedFileTypes = new List<string>();
            failedDirectories = new List<string>();
            isFullPath = false;
            isGroupByFileTypes = false;
            isSaveOutput = false;
        }

        protected virtual void SetArguments(string[] arguments)
        {
            int argumentCount = arguments.size();
            string nextArgument = "";
            for (int i = 0; i < argumentCount; i++)
            {
                // Coalesce (next argument, blank)
                nextArgument = i < argumentCount ? arguments[i + 1] : "";

                if (IsParameter(arguments[i]))
                {
                    SetProperty(arguments[i], nextArgument);
                }
            }
        }

        protected virtual bool IsParameter(string argument)
        {
            return argument[0] == '-';
        }

        protected virtual void SetProperty(string argument, string nextArgument)
        {
            switch(argument)
            {
                case "-fullpath":
                case "-fp":
                    isFullPath = true;
                    break;

                case "-groupbyfiletype":
                case "-gft":
                    isGroupByFileTypes = true;
                    break;

                case "-exclude":
                case "-e":
                    AddToExcludedFileTypes(nextArgument);
                    break;

                case "-save":
                case "-s":
                    isSaveOutput = true;
                    SaveDirectory = nextArgument;
                    break;

                default:
                    break;
            }
        }

        private virtual void AddToExcludedFileTypes(string parameters)
        {
            string[] excludedParameters = String.Split(',', parameters);
            foreach (string excludedParameter in excludedParameters)
            {
                ExcludedFileTypes.Add(excludedParameter);
            }
        }



    }

}

