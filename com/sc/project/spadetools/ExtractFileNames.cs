
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace com.sc.project.spadetools {
    
    class ExtractFileNames {

        #region Variables and Properties

        // Constants
        private readonly char DASH = '-';

        // Variables
        private bool isIncludeFilepaths;
        private bool useBlackList;
        private List<string> files;
        private List<string> fileTypes;
        private List<string> failedDirectories;
        private List<string> writtenDirectories;
        private List<string> excludedFiles;
        private List<Regex> patterns;
        private string _rootDirectory;
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
                patterns = new List<Regex>();
                isIncludeFilepaths = true;
                useBlackList = false;

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

                    case "--exclude":
                    case "-ex":
                        SetExcludedFiles(parameter);
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

        #region SetSaveFile
        private void SetSaveFile(string parameter)
        {
            // If directory is given, default the save file
            if (Directory.Exists(parameter))
            {
                SetSaveFile(parameter + "\\extractedFileNames.txt");
                return;
            }
            saveFile = parameter;
        }
        #endregion SetSaveFile

        #region SetExcludedFiles
        private void SetExcludedFiles(string parameters)
        {
            patterns.Clear();
            useBlackList = true;
            
            excludedFiles = parameters.Split("|").ToList();
            foreach (string excludedFile in excludedFiles)
            {
                SetPatterns(excludedFile);
            }
        }
        #endregion SetExcludedFiles

        #region SetPatterns
        private void SetPatterns(string parameter)
        {
            string regexBuild = "";
            // Handle the escape sequence
            string completePattern = parameter.Replace(@"\", @"\\");
            string[] splitPattern = completePattern.Trim().Split("*");
            int splitPatternCount = splitPattern.Length;
            
            if (splitPatternCount == 1)
            {
                patterns.Add(new Regex(@parameter));
                return;
            }

            for (int i = 0; i < splitPatternCount; i++)
            {
                // Asterisk at the very first
                if (i == 0 && String.IsNullOrEmpty(splitPattern[i]))
                {
                    regexBuild += "^";
                }
                // Asterisk at the very last
                else if (i == splitPatternCount - 1 && String.IsNullOrEmpty(splitPattern[i]))
                {
                    regexBuild += "$";
                }
                else if (i > 1 && i < splitPatternCount)
                {
                    regexBuild += ".*";
                }
                else
                {
                    regexBuild += splitPattern[i];
                }
            }

            patterns.Add(new Regex(regexBuild));
        }
        #endregion SetPatterns

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
                    if (useBlackList && IsBlackListed(internalFile))
                    {
                        continue;
                    }
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

        #region IsBlackListed 
        private bool IsBlackListed(string parameter)
        {
            foreach (string excludedFile in excludedFiles)
            {
                if (IsPatternMatch(excludedFile, parameter))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion IsBlackListed

        #region IsPatternMatch
        private bool IsPatternMatch(string mainPattern, string parameter)
        {
            string[] patterns = mainPattern.Split("*");
            foreach (string pattern in patterns)
            {

            }
            return true;
        }
        #endregion IsPatternMatch

        #region WriteIntoFile
        private void WriteIntoFile()
        {
            try
            {
                // If save file was never set, create default file under root directory
                if (String.IsNullOrEmpty(saveFile))
                {
                    SetSaveFile(RootDirectory);
                }
                
                writtenDirectories.Sort();
                // Close the file before writing
                using (File.Create(saveFile)) { }  
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

