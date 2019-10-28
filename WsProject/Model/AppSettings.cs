
namespace ApiRefibra.Model
{
    public class AppSettings
    {
        public string StorageConnectionString { get; set; }
        public string LoginFuseki { get; set; }
        public string PasswordFuseki { get; set; }
        public string MetaRefibra { get; set; }
        public double PageRank { get; set; }

        public string WikifierUrl { get; set; }
        public string WikifierKey { get; set; }
        public string NameCloudUpload { get; set; }
        public string PassUpload { get; set; }
        public string KeyUpload { get; set; }
        public string DiretoryUpload { get; set; }
    }
}
