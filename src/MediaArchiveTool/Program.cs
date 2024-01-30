// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text.Json;
using MediaArchiveTool;

public class Program
{
    public static async Task List(Options opt)
    {
        FileStream? OutputStream = null;

        if(!MediaArchiveTool.Helpers.Helper.CheckDirectory(opt.SourceFolder))
        {
            Console.WriteLine($"{opt.GetErrorMessagePrefix()} Source folder: {opt.SourceFolder} not found");
            Console.WriteLine(opt.GetInformationMessage());
            return;
        }
        if((OutputStream =  MediaArchiveTool.Helpers.Helper.CheckFileAndCreate(opt.OutputFile)) == null)
        {
            Console.WriteLine($"{opt.GetErrorMessagePrefix()} output file: {opt.OutputFile} can't be created");
            Console.WriteLine(opt.GetInformationMessage());
            return;
        }
        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, OutputStream,$"Start Browsing folder {opt.SourceFolder} ...");

        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, OutputStream,"Browsing done");

    }
    public static async Task Archive(Options opt)
    {
        FileStream? OutputStream = null;

        if(!MediaArchiveTool.Helpers.Helper.CheckDirectory(opt.SourceFolder))
        {
            Console.WriteLine($"{opt.GetErrorMessagePrefix()} Source folder: {opt.SourceFolder} not found");
            Console.WriteLine(opt.GetInformationMessage());
            return;
        }
        if(!MediaArchiveTool.Helpers.Helper.CheckDirectoryAndCreate(opt.DestinationFolder))
        {
            Console.WriteLine($"{opt.GetErrorMessagePrefix()} Destination folder: {opt.DestinationFolder} not found");
            Console.WriteLine(opt.GetInformationMessage());
            return;
        }
        if((OutputStream =  MediaArchiveTool.Helpers.Helper.CheckFileAndCreate(opt.OutputFile)) == null)
        {
            Console.WriteLine($"{opt.GetErrorMessagePrefix()} output file: {opt.OutputFile} can't be created");
            Console.WriteLine(opt.GetInformationMessage());
            return;
        }
        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, OutputStream,$"Start Archiving from folder {opt.SourceFolder} to {opt.DestinationFolder} ...");

        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, OutputStream,"Archiving done");
    }
    static async Task Main(string[] args)
    {


        Options opt = Options.ParseCommand(args);
        string error = opt.GetErrorMessage();
        if (string.IsNullOrEmpty(error))
        {
            if(opt.ArchiveAction == Options.Action.List)
            {
                await List(opt);
            }
            else if (opt.ArchiveAction == Options.Action.Archive)
            {
                await Archive(opt);
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


