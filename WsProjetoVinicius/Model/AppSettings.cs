using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiJenaFusekiRefibra.Model
{
    public class AppSettings
    {
        public string StorageConnectionString { get; set; }
        public string LoginFuseki { get; set; }
        public string PasswordFuseki { get; set; }
        public string MetaRefibra { get; set; }

        public string PageRank { get; set; }
    }
}
