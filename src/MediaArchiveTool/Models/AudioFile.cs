namespace MediaArchiveTool.Models
{
    public class AudioFile
    {
        public string path { get; set; } = string.Empty;
        public ulong size { get; set; } = 0;
        public DateTime creation_date { get; set; } = DateTime.Now;
    }
}
