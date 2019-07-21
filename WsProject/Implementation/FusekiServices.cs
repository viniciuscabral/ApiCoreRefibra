using ApiRefibra.Interface;
using ApiRefibra.Exceptions;
using ApiRefibra.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;


namespace ApiRefibra.Implementation
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
                    if(annotationObj.PageRank > _appSettings.PageRank)
                    {
                        listRdf.Add(rdf);
                    }
                    
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
            SparqlResultSet rset = (SparqlResultSet)fuseki.Query("SELECT ?item ?title ?img                               " +
                                                                "WHERE {                                                " +
                                                                "  ?item <http://metadadorefibra.ufpe/text> ?object .   " +
                                                                "  ?item <http://metadadorefibra.ufpe/title> ?title .   " +
                                                                "  ?item <http://metadadorefibra.ufpe/image> ?img       " +
                                                                "}");

            foreach (SparqlResult result in rset.Results)
            {
                listRdfBase.Add(new { item = result.Value("item").ToString()
                                    , title = result.Value("title").ToString()
                                     , image = result.Value("img").ToString()
                });
            }

            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }
        public IEnumerable<Object> GetAllItensRelation()
        {
           List<Object> listRdfBase = new List<Object>();
           try{
                VDS.RDF.Options.UriLoaderCaching = false;
                FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
                IGraph h = new Graph();
                fuseki.LoadGraph(h, (Uri)null);
               
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
            }
            catch(VDS.RDF.Storage.RdfStorageException) {
                    throw new FusekiException("Connetion database error.");
            }
            catch(VDS.RDF.Query.RdfQueryException) {
                     throw new FusekiException("Malformed query.");
            }
            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }
        public Object GetItemByName( string item)
        {
            List<string> texts =new List<string>();
            string image = "";
            string title = "";
            List<string> listRelationObject = new List<string>();
            IEnumerable<object> listRelationItem = new List<string>();
            try{                
                VDS.RDF.Options.UriLoaderCaching = false;
                FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
                IGraph h = new Graph();
                fuseki.LoadGraph(h, (Uri)null);

                string query = "select distinct ?relation ?obj " +   
                                "where { "+
                                "  "+ValidateNameItemRefibra(item)+ " ?relation ?obj " +
                                " } ";
              

                SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);            
                
                foreach (SparqlResult result in rset.Results)
                {                    
                    if (result.Value("relation").ToString().Contains("text"))
                    {
                        texts.Add(result.Value("obj").ToString());
                    }
                    if (result.Value("relation").ToString().Contains("title"))
                    {
                        title = result.Value("obj").ToString();
                    }
                    if (result.Value("relation").ToString().Contains("image"))
                    {
                        image = result.Value("obj").ToString();
                    }
                    if (result.Value("relation").ToString().Contains("relation"))
                    {
                        listRelationObject.Add(result.Value("obj").ToString());
                    }                    
                }
                listRelationItem = GetItensRelationByItemName(item);
            }
            catch(VDS.RDF.Storage.RdfStorageException) {
                    throw new FusekiException("Connetion database error.");
            }
            catch(VDS.RDF.Query.RdfQueryException) {
                     throw new FusekiException("Malformed query.");
            }

            VDS.RDF.Options.UriLoaderCaching = true;
            return new
            {
                Text = texts,
                Image = image,
                Title = title,
                ListRelation = listRelationObject,
                listRelationItem = listRelationItem
            }; 
        }
        public IEnumerable<Object> GetItensRelationByItemName(string itemName)
        {
            VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<Object> listRdfBase = new List<Object>();

            itemName = ValidateNameItemRefibra(itemName);
            var query = "select                                  " +
                        "distinct  ?item1 ?relation ?item2 ?obj  " +
                        "WHERE {                                 " +
                        "   ?item1 ?relation ?obj .              " +
                        "  ?item2 ?relation ?obj .               " +
                        "  filter ( ?item1 != ?item2) .          " +
                        " filter (?item1 = "+ itemName +")        "+
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
            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }
        public IEnumerable<Object> GetAllRelationsNames(){
             VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<string> listRelations = new List<string>();

            var query = "SELECT distinct ?object "+
                                "WHERE { "+
                                " ?subject <http://metadadorefibra.ufpe/relation> ?object "+
                                "}"; 

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);
            foreach (SparqlResult result in rset.Results)
            {
                listRelations.Add(                
                    result.Value("object").ToString()                   
                );
            }
            VDS.RDF.Options.UriLoaderCaching = true;
            return listRelations;
        }
        public IEnumerable<Object> GetItensByRelationName(string relationName){
             VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = new FusekiConnector(_appSettings.StorageConnectionString);
            IGraph h = new Graph();
            fuseki.LoadGraph(h, (Uri)null);
            List<object> listItens = new List<object>();

            var query = "SELECT ?item "+
                        "WHERE { "+
                        " ?item ?p <"+relationName+"> "+
                        " }"; 

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);
            foreach (SparqlResult result in rset.Results)
            {
                listItens.Add(GetItemByName(result.Value("item").ToString()));             
            }
            VDS.RDF.Options.UriLoaderCaching = true;
            return listItens;
        }
       
        #region Private methods
        private async Task<WikifierObj> ProcessarWikifier(string text)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userKey", "sqjrgxfcuvvbjijjscvsfsjugnerja");
            dict.Add("text", text);
            dict.Add("wikiDataClasses", "false");
            dict.Add("includeCosines", "false");
            dict.Add("support", "false");
            dict.Add("applyPageRankSqThreshold ", "false");
            dict.Add("pageRankSqThreshold", "0,6");

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
        private string ValidateNameItemRefibra(string itemName){
           itemName = string.IsNullOrEmpty(itemName) ? "" : itemName;
           itemName = itemName.Contains("<") ? itemName : "<"+itemName;
           itemName = itemName.Contains(">") ? itemName : itemName+">";
           return itemName.Contains(_appSettings.MetaRefibra) ? itemName : "<"+_appSettings.MetaRefibra + itemName +">";
        }
        #endregion
    }
}
