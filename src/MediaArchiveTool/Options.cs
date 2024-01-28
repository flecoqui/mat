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
            "mat --archive [--audio] [--picture] [--video] --source [source-folder]  --destination [destination-folder] [--verbose] \r\n" +
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
        public Action ArchiveAction = Action.Help;
        public string SourceFolder { get; set; } = string.Empty;

        public string DestinationFolder { get; set; } = string.Empty;
        public string OutputFile { get; set; } = string.Empty;
        public bool Audio { get; set; } = false;
        public bool Video { get; set; } = false;
        public bool Picture { get; set; } = false;

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
                                options.ArchiveAction = Action.List;
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
        public static Options CheckOptions(Options options)
        {
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
