using System.Text;
using System.Threading.Tasks;
using System.IO;
using MediaArchiveTool;
namespace MediaArchiveTool.Helpers
{
    public static class Helper
    {
        public static bool CheckDirectory(string path)
        {
            try
            {
                if(!System.IO.Directory.Exists(path)){
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
            }
            finally {}
            return false;
        } 
        public static bool CheckDirectoryAndCreate(string path)
        {
            try
            {
                if(!System.IO.Directory.Exists(path)){
                    DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                }
                return true;
            }
            catch (Exception)
            {
            }
            finally {}
            return false;
        }  
        public static FileStream? CheckFileAndCreate(string path)
        {
            try
            {
                FileStream createdFile = System.IO.File.Create(path, 4096, FileOptions.Asynchronous);
                return createdFile;
            }
            catch (Exception)
            {
            }
            finally {}
            return null;
        }  
        public static async Task<long> WriteFile(FileStream stream, string text)
        {
            try
            {
                byte[] bytesToWrite = Encoding.Unicode.GetBytes(text);
                await stream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
                return bytesToWrite.Length;
            }
            catch (Exception)
            {
            }
            finally {}
            return 0;
        }  

        public static async Task<long> Log(bool Verbose, FileStream? stream, string text)
        {
            long len = 0;
            if(Verbose){
                Console.WriteLine(text);
                len = 1;
            }
            if(stream != null)
                len = await Helper.WriteFile(stream,text);
            return len;
        }  

        public static FileInfo[]? GetFiles(bool Verbose, FileStream? stream, string folder, string[] extensions)
        {
            FileInfo[]? files = null;
            DirectoryInfo di = new DirectoryInfo(folder);
            try{
                if(di.Exists)
                {
                    files =
                        di.GetFiles()
                            .Where(f => extensions.Contains(f.Extension.ToLower()))
                            .ToArray();
                }
            }
            catch (Exception)
            {
            }
            finally {}
            return files;
        }
        public static DirectoryInfo[]? GetDirectories(bool Verbose, FileStream? stream, string folder)
        {
            DirectoryInfo[]? directories = null;
            DirectoryInfo di = new DirectoryInfo(folder);
            try{
                if(di.Exists)
                {
                    directories =
                        di.GetDirectories();
                }
            }
            catch (Exception)
            {
            }
            finally {}
            return directories;
        }
        public static async Task<(long,long)> BrowseFiles(bool Verbose, FileStream? stream, string folder, Options opt)
        {
            long FileCounter = 0; 
            long SizeCounter = 0;
            
            int counter = opt.SourceFolder.Count(f => f == '/' || f == '\\');
            counter = folder.Count(f => f == '/' || f == '\\') - counter;
            string prefix = string.Empty;
            for(int i = 0; i < counter; i++) prefix += " ";

            FileInfo[]? list = null;
            if(opt.Audio){
                list = MediaArchiveTool.Helpers.Helper.GetFiles(opt.Verbose, stream, folder, opt.AudioExtensionsArray);
                if(list != null)
                    foreach( FileInfo fi  in list)
                    {
                        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}File: {fi.Name} - {fi.Length} Bytes");
                        FileCounter += 1;
                        SizeCounter += fi.Length;
                    }
            }
            if(opt.Picture){
                list = MediaArchiveTool.Helpers.Helper.GetFiles(opt.Verbose, stream, folder, opt.PictureExtensionsArray);
                if(list != null)
                    foreach( FileInfo fi  in list)
                    {
                        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}File: {fi.Name} - {fi.Length} Bytes");
                        FileCounter += 1;
                        SizeCounter += fi.Length;
                    }
            }
            if(opt.Video){
                list = MediaArchiveTool.Helpers.Helper.GetFiles(opt.Verbose, stream, folder, opt.VideoExtensionsArray);
                if(list != null)
                    foreach( FileInfo fi  in list)
                    {
                        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}File: {fi.Name} - {fi.Length} Bytes");
                        FileCounter += 1;
                        SizeCounter += fi.Length;
                    }
            }
            DirectoryInfo[]? listdir = MediaArchiveTool.Helpers.Helper.GetDirectories(opt.Verbose, stream, folder);
            if(listdir != null)
                foreach( DirectoryInfo di  in listdir)
                {
                    await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}Folder: {di.Name}");
                    var (fCounter,sCounter) = await MediaArchiveTool.Helpers.Helper.BrowseFiles(Verbose, stream, di.FullName, opt);
                    FileCounter += fCounter;
                    SizeCounter += sCounter;
                }
            return (FileCounter,SizeCounter);
        }        
    }
}
