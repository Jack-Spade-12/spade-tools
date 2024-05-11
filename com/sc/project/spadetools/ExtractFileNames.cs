
using System.IO;
using System.Text;

namespace com.sc.project.spadetools {
    
    class ExtractFileNames {

        #region Variables and Properties

        // Constants
        private readonly char DASH = '-';

        // Variables
        private bool isIncludeFilepaths;
        private List<string> files;
        private List<string> fileTypes;
        private List<string> failedDirectories;
        private List<string> writtenDirectories;
        private string _rootDirectory;
        private string _saveDirectory;
        private string saveFile;

        // Properties
        public string RootDirectory
        {
            get
            {
                return _rootDirectory;
            }
            set
            {
                _rootDirectory = value;
            }
        }

        #endregion Variables and Properties

        #region Constructor

        public ExtractFileNames(string[] arguments)
        {
            try
            {
                // Initialize variables
                files = new List<string>();
                fileTypes = new List<string>();
                failedDirectories = new List<string>();
                writtenDirectories = new List<string>();
                isIncludeFilepaths = true;

                // Execute main instructions
                ProcessArguments(arguments);
                Process(RootDirectory);
                WriteIntoFile();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        #endregion Constructor

        #region Pre-Processes

        #region ProcessArguments
        private void ProcessArguments(string[] arguments)
        {
            try
            {
                int argumentsSize = arguments.Length;

                for (int i = 0; i < argumentsSize; i++)
                {
                    if (IsProcess(arguments[i]))
                    {
                        SetProperties(arguments[i], i + 1 < argumentsSize ? arguments[i + 1] : String.Empty);
                    }
                    // If first argument is not process, assume it is root directory
                    else if (i == 0)
                    {
                        SetProperties("--root", arguments[0]);
                        SetProperties("--save", RootDirectory + "\\extractedFileNames.txt");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion ProcessArguments

        #region IsProcess
        private bool IsProcess(string argument)
        {
            return argument[0] == DASH;
        }
        #endregion IsProcess

        #region SetProperties
        private void SetProperties(string process, string parameter)
        {
            try
            {
                switch (process)
                {
                    case "--filepath":
                    case "-fp":
                        SetIncludeFilepathProperty(parameter);
                        break;

                    case "--getdefault":
                    case "-gd":
                        GetDefaults(parameter);
                        break;

                    case "--setdefault":
                    case "-sd":
                        SetDefaults(parameter);
                        break;

                    case "--root":
                        SetRootDirectory(parameter);
                        break;

                    case "--save":
                        SetSaveFile(parameter);
                        break;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion SetProperties

        #region SetIncludeFilepathProperty
        private void SetIncludeFilepathProperty(string parameter)
        {
            switch (parameter.ToLower())
            {
                case "y":
                case "yes":
                case "true":
                    isIncludeFilepaths = true;
                    break;

                case "n":
                case "no":
                case "false":
                    isIncludeFilepaths = false;
                    break;

                default:
                    throw new ArgumentException("Set Filepath Property parameter is invalid. Valid values are: y, yes, true, n, no, false.");
            }
        }
        #endregion SetIncludeFilepathProperty

        #region GetDefaults
        private void GetDefaults(string parameter)
        {
            try
            {
                // use defaults if no defaults are used
                if (String.IsNullOrEmpty(parameter))
                {
                    SetDefaults(_rootDirectory + "\\spade-defaults.json");
                    GetDefaults(_rootDirectory + "\\spade-defaults.json");
                    return;
                }
                else if (!File.Exists(parameter))
                {
                    throw new FileNotFoundException("Error : default values directory " + parameter + "not found.");
                }

                // todo: process here to parse the defaults file
            }
            catch
            {
                throw;
            }
        }
        #endregion GetDefaults

        #region SetDefaults
        private void SetDefaults(string parameter)
        {
            try
            {
                // create the file if it does not exist
                if (!File.Exists(parameter))
                {
                    File.Create(parameter);
                }

                // todo: process here to create/overtwrite the defaults file
            }
            catch
            {
                throw;
            }
        }
        #endregion SetDefaults

        #region SetRootDirectory
        private void SetRootDirectory(string parameter)
        {
            // Validate that the root directory exists
            if (!Directory.Exists(parameter))
            {
                throw new DirectoryNotFoundException("Error : Root directory " + parameter + " not found.");
            }
            RootDirectory = parameter;
        }
        #endregion SetRootDirectory

        #region SetSaveDirectory
        private void SetSaveFile(string parameter)
        {
            // Validate that the save directory exists
            if (!File.Exists(parameter))
            {
                File.Create(parameter);
            }
            saveFile = parameter;
        }
        #endregion SetSaveDirectory

        #endregion Pre-Processes

        #region Main Process

        #region Process
        private void Process(string directory)
        {
            try
            {
                // Record all files in the directory
                foreach (string internalFile in Directory.GetFiles(directory))
                {
                    writtenDirectories.Add(isIncludeFilepaths ? internalFile : Path.GetFileName(internalFile));
                }

                // Recurse for each internal directory
                foreach (string internalDirectory in Directory.GetDirectories(directory))
                {
                    Process(internalDirectory);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Process

        #region WriteIntoFile
        private void WriteIntoFile()
        {
            try
            {
                writtenDirectories.Sort();
                File.WriteAllLines(saveFile, writtenDirectories);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion WriteIntoFile

        #endregion Main Process

    }

}

