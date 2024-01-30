using System.Text;
using System.Threading.Tasks;
using System.IO;
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

        public static async Task<long> Log(bool Verbose, FileStream stream, string text)
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
    }
}
