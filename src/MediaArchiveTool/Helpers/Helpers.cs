using System.Text;
using System.Threading.Tasks;
using System.IO;
using MediaArchiveTool;
using MediaArchiveTool.Helpers.ExifLib;
namespace MediaArchiveTool.Helpers
{
    public static class Helper
    {

        public static bool AreFilesIdenticalFast(string path1, string path2)
        {
            return AreFilesIdentical(path1, path2, AreStreamsIdenticalFast);
        }
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
        public static bool AreFilesIdentical(string path1, string path2, Func<Stream, Stream, bool> areStreamsIdentical)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1));

            if (path2 == null)
                throw new ArgumentNullException(nameof(path2));

            if (areStreamsIdentical == null)
                throw new ArgumentNullException(nameof(path2));

            if (!File.Exists(path1) || !File.Exists(path2))
                return false;

            using (var thisFile = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var valueFile = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (valueFile.Length != thisFile.Length)
                        return false;

                    if (!areStreamsIdentical(thisFile, valueFile))
                        return false;
                }
            }
            return true;
        }

        public static bool AreStreamsIdenticalFast(Stream stream1, Stream stream2)
        {
            if (stream1 == null)
                throw new ArgumentNullException(nameof(stream1));

            if (stream2 == null)
                throw new ArgumentNullException(nameof(stream2));

            const int bufsize = 80000; // 80000 is below LOH (85000)

            var tasks = new List<Task<bool>>();
            do
            {
                // consumes more memory (two buffers for each tasks)
                var buffer1 = new byte[bufsize];
                var buffer2 = new byte[bufsize];

                int read1 = stream1.Read(buffer1, 0, buffer1.Length);
                if (read1 == 0)
                {
                    int read3 = stream2.Read(buffer2, 0, 1);
                    if (read3 != 0) // not eof
                        return false;

                    break;
                }

                // both stream read could return different counts
                int read2 = 0;
                do
                {
                    int read3 = stream2.Read(buffer2, read2, read1 - read2);
                    if (read3 == 0)
                        return false;

                    read2 += read3;
                }
                while (read2 < read1);

                // consumes more cpu
                var task = Task.Run(() =>
                {
                    return IsSame(buffer1, buffer2);
                });
                tasks.Add(task);
            }
            while (true);

            Task.WaitAll(tasks.ToArray());
            return !tasks.Any(t => !t.Result);
        }
        public static bool IsSame(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null)
                throw new ArgumentNullException(nameof(bytes1));

            if (bytes2 == null)
                throw new ArgumentNullException(nameof(bytes2));

            if (bytes1.Length != bytes2.Length)
                return false;

            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }
            return true;
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

        public static async Task<long> Log(bool verbose, FileStream? stream, string text)
        {
            long len = 0;
            if(verbose){
                Console.WriteLine(text);
                len = 1;
            }
            if(stream != null)
                len = await Helper.WriteFile(stream,text);
            return len;
        }  

        public static FileInfo[]? GetFiles(bool verbose, FileStream? stream, string folder, string[] extensions)
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
        public static DirectoryInfo[]? GetDirectories(bool verbose, FileStream? stream, string folder)
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
        public static string GetDestinationFolder(string folder, DateTime date, Options.Split split)
        {
            string result = string.Empty;
            if (split == Options.Split.Day)
            {
                
                result = $"{folder}{Path.DirectorySeparatorChar}{date.ToString("yyyy_MM_dd")}"; 
            }
            else if (split == Options.Split.Month)
            {
                result = $"{folder}{Path.DirectorySeparatorChar}{date.ToString("yyyy_MM")}"; 
            }
            else 
            {
                result = $"{folder}{Path.DirectorySeparatorChar}{date.ToString("yyyy")}"; 
            }
            return result;
        }
        public static string GetNewDestinationFile(string dest, int index)
        {
            string result = dest;
            string? directory = Path.GetDirectoryName(dest);
            string extension = Path.GetExtension(dest);
            string filename = Path.GetFileNameWithoutExtension(dest);

            return $"{directory}{Path.PathSeparator}{filename}-{index}.{extension}";
        }
        public static async Task CopyFile(bool verbose, FileStream? stream, string source, string destination)
        {
            await MediaArchiveTool.Helpers.Helper.Log(verbose, stream, $"Copying file {source} to {destination}");
            string? directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(directory)) {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    bool destinationFileExists = true;
                    int index = 1;
                    while (destinationFileExists == true)
                    {
                        if (File.Exists(destination) == true)
                        {
                            if (AreFilesIdenticalFast(source, destination) == true)
                            {
                                await MediaArchiveTool.Helpers.Helper.Log(verbose, stream, $"Copying file {source} to {destination} not necessary destination file already exists");
                                return;
                            }
                            else
                            {
                                destination = GetNewDestinationFile(destination,index++);
                            }
                        }
                        else
                            destinationFileExists = false;
                    }
                    

                    File.Copy(source, destination, true);
                    await MediaArchiveTool.Helpers.Helper.Log(verbose, stream, $"Copying file {source} to {destination} done");
                }
                catch (Exception ex) {
                    await MediaArchiveTool.Helpers.Helper.Log(verbose, stream, $"Exception while copying file {source} to {destination}: {ex.Message}");
                }
            }
        }
        public static async Task<(long,long)> BrowseOrCopyFiles(bool copy, bool verbose, FileStream? stream, string folder, Options opt)
        {
            long FileCounter = 0; 
            long SizeCounter = 0;
            
            int counter = opt.SourceFolder.Count(f => f == Path.DirectorySeparatorChar);
            counter = folder.Count(f => f == Path.DirectorySeparatorChar) - counter;
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
                        DateTime CreationDateTime = fi.CreationTime > fi.LastWriteTime ? fi.LastWriteTime : fi.CreationTime;
                        string? Manufacturer = "Unknown";
                        string? CameraModel = "Unknown";
                        // Instantiate the reader
                        try
                        {
                            using (ExifReader reader = new ExifReader($"{folder}{Path.DirectorySeparatorChar}{fi.Name}"))
                            {
                                // Extract the tag data using the ExifTags enumeration
                                DateTime datePictureTaken;
                                
                                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, 
                                                                out datePictureTaken))
                                {
                                    CreationDateTime = datePictureTaken;
                                }
                                string? Make = "Unknown";
                                if (reader.GetTagValue<String>(ExifTags.Make, 
                                                                out Make))
                                {
                                    Manufacturer = Make;
                                }                                
                                string? Camera = "Unknown";
                                if (reader.GetTagValue<String>(ExifTags.Model, 
                                                                out Camera))
                                {
                                    CameraModel = Camera;
                                }                                
                            }
                        }
                        catch(Exception ex)
                        {
                            await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}Exception while analyzing File: {fi.Name} - {fi.Length} Bytes: {ex.Message}");
                        }                    
                        string destination = GetDestinationFolder(opt.DestinationFolder, CreationDateTime, opt.ArchiveSplit);
                        if(copy == false)
                            await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}File: {fi.Name} - {fi.Length} Bytes - Date: {CreationDateTime} - Manufacturer: {Manufacturer}/{CameraModel} - Destination: {destination}");
                        if(copy)
                            await CopyFile(opt.Verbose, stream, $"{folder}{Path.DirectorySeparatorChar}{fi.Name}",$"{destination}{Path.DirectorySeparatorChar}{fi.Name}");

                        FileCounter += 1;
                        SizeCounter += fi.Length;
                    }
            }
            if(opt.Video){
                list = MediaArchiveTool.Helpers.Helper.GetFiles(opt.Verbose, stream, folder, opt.VideoExtensionsArray);
                if(list != null)
                    foreach( FileInfo fi  in list)
                    {
                        DateTime CreationDateTime = fi.CreationTime > fi.LastWriteTime ? fi.LastWriteTime : fi.CreationTime;
                        await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}File: {fi.Name} - {fi.Length} Bytes - Date {CreationDateTime}");

                        string destination = GetDestinationFolder(opt.DestinationFolder, CreationDateTime, opt.ArchiveSplit);
                        if (copy == false)
                            await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream, $"{prefix}File: {fi.Name} - {fi.Length} Bytes - Date: {CreationDateTime} - Destination: {destination}");
                        if (copy)
                            await CopyFile(opt.Verbose, stream, $"{folder}{Path.DirectorySeparatorChar}{fi.Name}", $"{destination}{Path.DirectorySeparatorChar}{fi.Name}");


                        FileCounter += 1;
                        SizeCounter += fi.Length;
                    }
            }
            DirectoryInfo[]? listdir = MediaArchiveTool.Helpers.Helper.GetDirectories(opt.Verbose, stream, folder);
            if(listdir != null)
                foreach( DirectoryInfo di  in listdir)
                {
                    await MediaArchiveTool.Helpers.Helper.Log(opt.Verbose, stream,$"{prefix}Folder: {di.Name}");
                    var (fCounter,sCounter) = await MediaArchiveTool.Helpers.Helper.BrowseOrCopyFiles(copy, verbose, stream, di.FullName, opt);
                    FileCounter += fCounter;
                    SizeCounter += sCounter;
                }
            return (FileCounter,SizeCounter);
        }        
    }
}
