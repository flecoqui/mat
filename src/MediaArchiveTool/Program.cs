// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text.Json;
using MediaArchiveTool;

public class Program
{
    public static void List(Options opt)
    {
    }
    public static void Archive(Options opt)
    {
    }
    static async Task Main(string[] args)
    {


        Options opt = Options.ParseCommand(args);
        string error = opt.GetErrorMessage();
        if (string.IsNullOrEmpty(error))
        {
            if(opt.ArchiveAction == Options.Action.List)
            {
                List(opt);
            }
            else if (opt.ArchiveAction == Options.Action.Archive)
            {
                List(opt);
            }
            else if (opt.ArchiveAction == Options.Action.Help)
            {
                Console.WriteLine(opt.GetInformationMessage());
            }
        }
        else
        {
            Console.WriteLine(opt.GetErrorMessagePrefix() + opt.GetErrorMessage());
            Console.WriteLine(opt.GetInformationMessage());
        }
    }

}


