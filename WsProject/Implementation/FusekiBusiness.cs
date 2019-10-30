using ApiRefibra.Interface;
using ApiRefibra.Exceptions;
using ApiRefibra.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ApiRefibra.Implementation
{
    public class FusekiBusiness : IFusekiBusiness
    {

        private AppSettings _appSettings { get; set; }
        private readonly string dataBaseConnection = Environment.GetEnvironmentVariable("DATABASE_FUSEKI_CONNECTION");
        private readonly IWikifierBusiness _wikifierBusiness;
        private IGraph graph = new Graph();

        public FusekiBusiness(IOptions<AppSettings> settings, IWikifierBusiness wikifierBusiness)
        {
            _appSettings = settings.Value;
            _wikifierBusiness = wikifierBusiness;
        }

        public List<string> GetDataSetNames()
        {
            string dataBaseAutentication = Environment.GetEnvironmentVariable("DATABASE_AUTENTICATION");
            string baseUrl = dataBaseConnection + "/$/datasets";
            List<string> dataSetName = new List<string>();
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            request.Headers.Add(
                HttpRequestHeader.Authorization.ToString(), dataBaseAutentication);
            var result = client.SendAsync(request).Result;

            using (HttpContent content = result.Content)
            {
                string data = content.ReadAsStringAsync().Result;

                if (!String.IsNullOrEmpty(data))
                {
                    var list = JObject.Parse(data)["datasets"].ToObject<List<object>>();
                    list.ForEach(x =>
                    {
                        dataSetName.Add(JObject.Parse(x.ToString())["ds.name"].ToString().Replace("/", ""));
                    });
                }
                else
                    dataSetName.Add("Refibra");
            }
            return dataSetName;
        }
        public List<RDFModel> RegisterItem(ItemRefibraModel item, string dataSet)
        {
            WikifierObjModel wikifierObj = _wikifierBusiness.ProcessarWikifier(item.Text);
            List<RDFModel> listRdf = new List<RDFModel>();
            if (wikifierObj != null)
            {
                RDFModel rdf = null;
                wikifierObj.Annotations.Where(x => x.PageRank >= _appSettings.PageRank)
                    .OrderByDescending(x => x.PageRank).ToList().ForEach(y =>
                                {
                                    rdf = new RDFModel();
                                    rdf.Subject = _appSettings.MetaRefibra + item.Name;
                                    rdf.Predicate = _appSettings.MetaRefibra + "relation";
                                    rdf.Object = y.Url;
                                    listRdf.Add(rdf);
                                });


                rdf = new RDFModel();
                rdf.Subject = _appSettings.MetaRefibra + item.Name;
                rdf.Predicate = _appSettings.MetaRefibra + "text";
                rdf.Object = item.Text;
                listRdf.Add(rdf);

                rdf = new RDFModel();
                rdf.Subject = _appSettings.MetaRefibra + item.Name;
                rdf.Predicate = _appSettings.MetaRefibra + "title";
                rdf.Object = item.Name;
                listRdf.Add(rdf);

                InsertTriples(listRdf, dataSet);
                UploadBase64Images(item.Name, item.Image);

                return listRdf;
            }
            else
            {
                return listRdf;
            }

        }
        public IEnumerable<Object> GetAllItens(string dataSet)
        {

            VDS.RDF.Options.UriLoaderCaching = false;
            List<Object> listRdfBase = new List<Object>();
            FusekiConnector fuseki = CreateFusekiConnector(dataSet);

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query("SELECT ?item ?title ?img                               " +
                                                                "WHERE {                                                " +
                                                                "  ?item <http://metadadorefibra.ufpe/text> ?object .   " +
                                                                "  ?item <http://metadadorefibra.ufpe/title> ?title .   " +
                                                                "}");

            foreach (SparqlResult result in rset.Results)
            {
                listRdfBase.Add(new
                {
                    item = result.Value("item").ToString(),
                    title = result.Value("title").ToString()
                });
            }

            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }
        public Object GetItemByName(string item, string dataSet)
        {
            List<string> texts = new List<string>();
            string image = "";
            string title = "";
            List<string> listRelationObject = new List<string>();
            IEnumerable<object> listRelationItem = new List<string>();
            try
            {
                VDS.RDF.Options.UriLoaderCaching = false;

                FusekiConnector fuseki = CreateFusekiConnector(dataSet);


                string query = "select distinct ?relation ?obj " +
                                "where { " +
                                "  " + ValidateNameItemRefibra(item) + " ?relation ?obj " +
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
                listRelationItem = GetItensRelationByItemName(item, dataSet);
            }
            catch (VDS.RDF.Storage.RdfStorageException)
            {
                throw new FusekiException("Connetion database error.");
            }
            catch (VDS.RDF.Query.RdfQueryException)
            {
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

        #region RELATIONS 
        public IEnumerable<Object> GetAllItensRelation(string dataSet)
        {
            List<Object> listRdfBase = new List<Object>();
            try
            {
                VDS.RDF.Options.UriLoaderCaching = false;
                FusekiConnector fuseki = CreateFusekiConnector(dataSet);

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
            catch (VDS.RDF.Storage.RdfStorageException)
            {
                throw new FusekiException("Connetion database error.");
            }
            catch (VDS.RDF.Query.RdfQueryException)
            {
                throw new FusekiException("Malformed query.");
            }
            VDS.RDF.Options.UriLoaderCaching = true;
            return listRdfBase;
        }
        public IEnumerable<Object> GetItensRelationByItemName(string itemName, string dataSet)
        {
            VDS.RDF.Options.UriLoaderCaching = false;

            FusekiConnector fuseki = CreateFusekiConnector(dataSet);
            List<Object> listRdfBase = new List<Object>();

            itemName = ValidateNameItemRefibra(itemName);
            var query = "select                                  " +
                        "distinct  ?item1 ?relation ?item2 ?obj  " +
                        "WHERE {                                 " +
                        "   ?item1 ?relation ?obj .              " +
                        "  ?item2 ?relation ?obj .               " +
                        "  filter ( ?item1 != ?item2) .          " +
                        " filter (?item1 = " + itemName + ")        " +
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
        public IEnumerable<Object> GetAllRelationsNames(string dataSet)
        {
            VDS.RDF.Options.UriLoaderCaching = false;

            FusekiConnector fuseki = CreateFusekiConnector(dataSet);

            List<string> listRelations = new List<string>();

            var query = "SELECT distinct ?object " +
                                "WHERE { " +
                                " ?subject <http://metadadorefibra.ufpe/relation> ?object " +
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
        public IEnumerable<Object> GetItensByRelationName(string relationName, string dataSet)
        {
            VDS.RDF.Options.UriLoaderCaching = false;
            FusekiConnector fuseki = CreateFusekiConnector(dataSet);
            List<object> listItens = new List<object>();
            
            var query = "SELECT ?item " +
                        "WHERE { " +
                        " ?item ?p <" + relationName + "> " +
                        " }";

            SparqlResultSet rset = (SparqlResultSet)fuseki.Query(query);
            foreach (SparqlResult result in rset.Results)
            {
                listItens.Add(GetItemByName(result.Value("item").ToString(), dataSet));
            }
            VDS.RDF.Options.UriLoaderCaching = true;
            return listItens;
        }
        #endregion

        #region Private methods

        private FusekiConnector CreateFusekiConnector(string dataSet)
        {            
            string dataBaseLogin = Environment.GetEnvironmentVariable("DATABASE_FUSEKI_LOGIN");
            string dataBasePass = Environment.GetEnvironmentVariable("DATABASE_FUSEKI_PASS");

            FusekiConnector fuseki = new FusekiConnector(dataBaseConnection + $"/{dataSet}/data");
            fuseki.SetCredentials(dataBaseLogin, dataBasePass);
            graph = new Graph();
            fuseki.LoadGraph(graph, (Uri)null);
            return fuseki;
        }
        private void InsertTriples(IEnumerable<RDFModel> rdfs, string dataSet)
        {
            try
            {
                FusekiConnector fuseki = CreateFusekiConnector(dataSet);
                List<Triple> ts = new List<Triple>();
                Triple triple = null;
                foreach (RDFModel rdf in rdfs)
                {
                    if (rdf.Object.Contains("http"))
                    {
                        triple = new Triple(graph.CreateUriNode(new Uri(rdf.Subject))
                          , graph.CreateUriNode(new Uri(rdf.Predicate))
                           , graph.CreateUriNode(new Uri(rdf.Object)));
                    }
                    else
                    {
                        triple = new Triple(graph.CreateUriNode(new Uri(rdf.Subject))
                      , graph.CreateUriNode(new Uri(rdf.Predicate))
                       , graph.CreateLiteralNode(rdf.Object));
                    }
                    ts.Add(triple);
                }

                fuseki.UpdateGraph(graph.BaseUri, ts, null);
            }
            catch (VDS.RDF.Storage.RdfStorageException)
            {
                throw new FusekiException("Connetion database error.");
            }
            catch (VDS.RDF.Query.RdfQueryException)
            {
                throw new FusekiException("Malformed query.");
            }
        }
        private string ValidateNameItemRefibra(string itemName)
        {
            itemName = string.IsNullOrEmpty(itemName) ? "" : itemName;
            itemName = itemName.Contains("<") ? itemName : "<" + itemName;
            itemName = itemName.Contains(">") ? itemName : itemName + ">";
            return itemName.Contains(_appSettings.MetaRefibra) ? itemName : "<" + _appSettings.MetaRefibra + itemName + ">";
        }
        private void UploadBase64Images(string imgName, string imgBase64)
        {
            if (!String.IsNullOrEmpty(imgName) && !String.IsNullOrEmpty(imgBase64)) { 
                Account account = new Account(
                          _appSettings.NameCloudUpload,
                          _appSettings.PassUpload,
                          _appSettings.KeyUpload);

                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imgBase64),
                    PublicId = $"{_appSettings.DiretoryUpload}/{imgName}"
                };
                 var uploadResult = cloudinary.UploadAsync(uploadParams).Result;
            }
        }

        #endregion
    }
}
