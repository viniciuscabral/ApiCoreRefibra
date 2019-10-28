using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiRefibra.Interface;
using ApiRefibra.Model;
using Microsoft.Extensions.Options;

namespace ApiRefibra.Implementation
{
    public class WikifierBusiness : IWikifierBusiness
    {
        private AppSettings _appSettings { get; set; }
        public WikifierBusiness(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public WikifierObjModel ProcessarWikifier(string text)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userKey", _appSettings.WikifierKey);
            dict.Add("text", text);
            dict.Add("wikiDataClasses", "false");
            dict.Add("includeCosines", "false");
            dict.Add("support", "false");
            dict.Add("applyPageRankSqThreshold ", "false");

            HttpClient client = new HttpClient();
            var response = client.PostAsync(_appSettings.WikifierUrl
                , new FormUrlEncodedContent(dict)).Result;

            if (response.IsSuccessStatusCode)
            {
                WikifierObjModel wikifierObjs = response.Content.ReadAsAsync<WikifierObjModel>().Result;
                return wikifierObjs;
            }
            else
            {
                return null;
            }
        }
    }
}
