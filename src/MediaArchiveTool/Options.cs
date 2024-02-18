using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaArchiveTool
{
    public class Options
    {
        private string ErrorMessagePrefix = "MediaArchiveTool Error: \r\n";
        private string InformationMessage = "MediaArchiveTool:\r\n" + "Version: 1.0.0 \r\n" + "Syntax:\r\n" +
            "mat --archive [--audio] [--picture] [--video] --source [source-folder]  --destination [destination-folder] [--year|--month|--day] [--verbose] \r\n" +
            "mat --list  [--audio] [--picture] [--video] --source [source-folder]  --destination [destination-folder] [--verbose] \r\n" +
            "mat --help";

        private string ErrorMessage = string.Empty;

        public enum Action
        {
            None = 0,
            Help,
            List,
            Archive,
        }
        public enum Split
        {
            Day = 0,
            Month,
            Year        }
        public Action ArchiveAction = Action.Help;
        public Split ArchiveSplit = Split.Day;
        public string SourceFolder { get; set; } = string.Empty;

        public string DestinationFolder { get; set; } = string.Empty;
        public string OutputFile { get; set; } = string.Empty;
        public bool Audio { get; set; } = false;
        public string AudioExtensions { get; set; } = "aac;mp3;wma;flac;m4a";
        public string[] AudioExtensionsArray { get; set; } = new string[0];
        public bool Video { get; set; } = false;
        public string VideoExtensions { get; set; } = "mp4;wmv;mov";
        public string[] VideoExtensionsArray { get; set; }  = new string[0];
        public bool Picture { get; set; } = false;
        public string PictureExtensions { get; set; } = "heic;jpg;png;raw";
        public string[] PictureExtensionsArray { get; set; }  = new string[0];
        public bool Verbose { get; set; } = false;

        public string GetErrorMessagePrefix()
        {
            return ErrorMessagePrefix;
        }
        public string GetErrorMessage()
        {
            return ErrorMessage;
        }
        public string GetInformationMessage()
        {
            return InformationMessage;
        }
        public static Options ParseCommand(string[] args)
        {
            Options options = new Options();
            try
            {
                if (args != null)
                {

                    int i = 0;
                    if (args.Length == 0)
                    {
                        options.ErrorMessage = "No parameter in the command line";
                        return options;
                    }
                    while ((i < args.Length) && (string.IsNullOrEmpty(options.ErrorMessage)))
                    {
                        switch (args[i++])
                        {

                            case "--help":
                                options.ArchiveAction = Action.Help;
                                break;
                            case "--list":
                                options.ArchiveAction = Action.List;
                                break;
                            case "--archive":
                                options.ArchiveAction = Action.Archive;
                                break;
                            case "--audio":
                                options.Audio = true;
                                break;
                            case "--video":
                                options.Video = true;
                                break;
                            case "--picture":
                                options.Picture = true;
                                break;
                            case "--day":
                                options.ArchiveSplit = Split.Day;
                                break;
                            case "--month":
                                options.ArchiveSplit = Split.Month;
                                break;
                            case "--year":
                                options.ArchiveSplit = Split.Year;
                                break;
                            case "--verbose":
                                options.Verbose = true;
                                break;                                
                            case "--source":
                                if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                    options.SourceFolder = args[i++];
                                else
                                    options.ErrorMessage = "Source Folder not set";
                                break;
                            case "--destination":
                                if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                    options.DestinationFolder = args[i++];
                                else
                                    options.ErrorMessage = "Destination Folder name not set";
                                break;                                
                            case "--outputfile":
                                if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                    options.OutputFile = args[i++];
                                else
                                    options.ErrorMessage = "Output file not set";
                                break;                                
                            default:
                                if ((args[i - 1].ToLower() == "dotnet") ||
                                    (args[i - 1].ToLower() == "exportlogs.dll") ||
                                    (args[i - 1].ToLower() == "exportlogs.exe"))
                                    break;
                                options.ErrorMessage = "wrong parameter: " + args[i - 1];
                                return options;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                options.ErrorMessage = "Exception while analyzing the options: " + ex.Message;
                return options;
            }

            if (!string.IsNullOrEmpty(options.ErrorMessage))
            {
                return options;
            }
            return CheckOptions(options);

        }
        public static string[] GetExtensionsArrays(string extensions)
        {
            string[] result = new string[0];
            if(!string.IsNullOrEmpty(extensions))
            {
                result = extensions.Split(';');
                for(int i = 0 ; i < result.Length; i++)
                {
                    if(!result[i].StartsWith("."))
                        result[i] = "."+result[i];
                }
            }
            return result;
        }
        public static Options CheckOptions(Options options)
        {
            options.AudioExtensionsArray = GetExtensionsArrays(options.AudioExtensions);
            options.VideoExtensionsArray = GetExtensionsArrays(options.VideoExtensions);
            options.PictureExtensionsArray = GetExtensionsArrays(options.PictureExtensions);
            if (options.ArchiveAction == Action.Help)
            {
                return options;
            }
            else if (options.ArchiveAction == Action.List)
            {
                if (!string.IsNullOrEmpty(options.SourceFolder)&&
                    ( options.Audio || options.Video || options.Picture ))
                {
                    return options;
                }
                else
                {
                    options.ErrorMessage = "Missing parameters for List feature";
                }
            }
            else if (options.ArchiveAction == Action.Archive)
            {
                if (!string.IsNullOrEmpty(options.SourceFolder)&&
                    !string.IsNullOrEmpty(options.DestinationFolder)&&
                    ( options.Audio || options.Video || options.Picture ))
                {
                    return options;
                }
                else
                {
                    options.ErrorMessage = "Missing parameters for Archive feature";
                }
            }
            else
            {
                options.ErrorMessage = "Action not set: --list --archive or --help";
            }
            return options;
        }
    }
}
