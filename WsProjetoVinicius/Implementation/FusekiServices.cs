using ApiJenaFusekiRefibra.Interface;
using ApiJenaFusekiRefibra.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace ApiJenaFusekiRefibra.Implementation
{
    public class FusekiServices : IFusekiServices
    {
        private readonly HttpClient client = new HttpClient();
        private AppSettings _appSettings { get; set; }
        public FusekiServices(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public async Task<List<RDF>> RegisterItem(Item item)
        {

            WikifierObj wikifierObj = await ProcessarWikifier(item.Text);
            List<RDF> listRdf = new List<RDF>();
            if(wikifierObj != null)
            {
                RDF rdf = null;
                foreach(AnnotationObj annotationObj in wikifierObj.Annotations)
                {
                    rdf = new RDF();
                    rdf.Subject = _appSettings.MetaRefibra + item.Name;
                    rdf.Predicate = _appSettings.MetaRefibra + "relation";
                    rdf.Object = annotationObj.Url;
                    listRdf.Add(rdf);
                }

                rdf = new RDF();
                rdf.Subject = _appSettings.MetaRefibra + item.Name;
                rdf.Predicate = _appSettings.MetaRefibra + "text"; 
                rdf.Object = item.Text;
                listRdf.Add(rdf);

                rdf = new RDF();
                rdf.Subject = _appSettings.MetaRefibra + item.Name;
                rdf.Predicate = _appSettings.MetaRefibra + "image";
                rdf.Object = item.Image;
                listRdf.Add(rdf);

                rdf = new RDF();
                rdf.Subject = _appSettings.MetaRefibra + item.Name;
                rdf.Predicate = _appSettings.MetaRefibra + "title";
                rdf.Object = item.Name;                
                listRdf.Add(rdf);

                InsertTriples(listRdf);

                return listRdf;
            }
            else{
                return null;
            }
            
        }     

        public IEnumerable<Object> GetAllItens()
        {

            VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<Object> listRdfBase = new List<Object>();
            SparqlResultSet rset = (SparqlResultSet)fuseki.Query("SELECT ?item ?title ?img                             " +
                                                            "WHERE {                                                      " +
                                                            "  ?item <http://metadadorefibra.ufpe/text> ?object           " +
                                                            "  {                                                          " +
                                                            "  select ?title                                              " +
                                                            "  where {  ?item <http://metadadorefibra.ufpe/title> ?title }" +
                                                            "  }                                                          " +
                                                            "  {                                                          " +
                                                            "  select ?img                                                " +
                                                            "  where {  ?item <http://metadadorefibra.ufpe/image> ?img }  " +
                                                            "  }                                                          " +
                                                            "}");

            foreach (SparqlResult result in rset.Results)
            {
                listRdfBase.Add(new { item = result.Value("item").ToString()
                                    , title = result.Value("title").ToString()
                                     , image = result.Value("img").ToString()
                });
            }

            //foreach (SparqlResult result in rset.Results)
            //{

            //    listRdfBase.Add(result.Value("subject").ToString());
            //}

            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }

        public IEnumerable<Object> GetItensByName()
        {
            VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<Object> listRdfBase = new List<Object>();

            string query = "PREFIX ref: <http://metadadorefibra.ufpe/>	  " +
                            "select distinct ?s ?valor " +
                            "where{ " +
                            "{ " +
                            "    select ?s ?p ?o " +
                            "  where {  ?s ?p ?o filter(regex(?o,'','i')) } " +
                            "} " +
                            "{ " +
                            "    select ?s  ?valor " +
                            "    where{  ?s ref:title ?valor } " +
                            "} }";

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);

            foreach (SparqlResult result in rset.Results)
            {
                listRdfBase.Add(result);
            }

            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }

        public IEnumerable<Object> GetItensRelation()
        {
            VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<Object> listRdfBase = new List<Object>();

            var query = "select                                  " +
                        "distinct  ?item1 ?relation ?item2 ?obj  " +
                        "WHERE {                                 " +
                        "   ?item1 ?relation ?obj .              " +
                        "  ?item2 ?relation ?obj .               " +
                        "  filter ( ?item1 != ?item2) .          " +
                        "  filter (!isLiteral(?obj)) .           " +
                        "}                                       " +
                        "group by  ?item1 ?relation ?item2 ?obj  "; 

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);
            foreach (SparqlResult result in rset.Results)
            {
                listRdfBase.Add(new
                {
                    item1 = result.Value("item1").ToString(),
                    relation = result.Value("relation").ToString(),
                    item2 = result.Value("item2").ToString(),
                    obj = result.Value("obj").ToString()
                });
            }

            //foreach (SparqlResult result in rset.Results)
            //{

            //    listRdfBase.Add(result);
            //}

            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }

        #region Privates methods
        //
        private async Task<WikifierObj> ProcessarWikifier(string text)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userKey", "sqjrgxfcuvvbjijjscvsfsjugnerja");
            dict.Add("text", text);
            dict.Add("wikiDataClasses", "false");
            dict.Add("includeCosines", "false");
            dict.Add("support", "false");
            dict.Add("applyPageRankSqThreshold ", "false");
            dict.Add("pageRankSqThreshold", "0.6");

            var response = await client.PostAsync("http://www.wikifier.org/annotate-article", new FormUrlEncodedContent(dict));

            if (response.IsSuccessStatusCode)
            {
                WikifierObj wikifierObjs = await response.Content.ReadAsAsync<WikifierObj>();
                return wikifierObjs;
            }
            else
            {
                return null;
            }
        }
        //
        private void InsertTriples(IEnumerable<RDF> rdfs)
        {
            try
            {
                FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
                fuseki.SetCredentials(_appSettings.LoginFuseki,_appSettings.PasswordFuseki);
                IGraph g = new Graph();
                fuseki.LoadGraph(g, (Uri)null);
                List<Triple> ts = new List<Triple>();

                Triple triple = null;
                foreach (RDF rdf in rdfs)
                {
                    if (rdf.Object.Contains("http"))
                    {
                        triple = new Triple(g.CreateUriNode(new Uri(rdf.Subject))
                          , g.CreateUriNode(new Uri(rdf.Predicate))
                           , g.CreateUriNode(new Uri(rdf.Object)));
                    }
                    else
                    {
                        triple = new Triple(g.CreateUriNode(new Uri(rdf.Subject))
                      , g.CreateUriNode(new Uri(rdf.Predicate))
                       , g.CreateLiteralNode(rdf.Object));
                    }
                    ts.Add(triple);
                }

                fuseki.UpdateGraph(g.BaseUri, ts, null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //
        #endregion
    }
}
