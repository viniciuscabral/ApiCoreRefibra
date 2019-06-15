using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiJenaFusekiRefibra.Model
{

    public class WikifierObj
    {
        public List<AnnotationObj> Annotations { get; set; }
    }

    public class AnnotationObj
    {
       public string Title { get; set; }
       public string Url { get; set; }
       public string Lang { get; set; }
       public double PageRank { get; set; }
       public string SecLang { get; set; }
       public string SecTitle { get; set; }
       public string SecUrl { get; set; }
       public string WikiDataItemId { get; set; }
       //public string DbPediaTypes { get; set; }
       public string DbPediaIri { get; set; }
       public string SupportLen { get; set; }
    }
}
