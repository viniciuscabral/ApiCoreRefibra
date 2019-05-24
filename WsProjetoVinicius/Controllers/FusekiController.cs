using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ApiJenaFusekiRefibra.Implementation;
using ApiJenaFusekiRefibra.Interface;
using ApiJenaFusekiRefibra.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace ApiJenaFusekiRefibra.Controllers
{
    [ApiController]
    public class FusekiController : Controller
    {
        private IFusekiServices _fusekiService;        
        public FusekiController(IFusekiServices fusekiService)
        {
            _fusekiService = fusekiService;
        }

        [HttpGet]
        [Route("GetAllItens")]
        public IActionResult GetAllItens() 
        {      
            return Ok(_fusekiService.GetAllItens());
        }

        [HttpGet]
        [Route("ItensByName")]
        public IActionResult ItensByName()
        {
            return Ok(_fusekiService.GetItensByName());
        }

        [HttpGet]
        [Route("ItensRelation")]
        public IActionResult ItensRelation()
        {
            return Ok(_fusekiService.GetItensRelation());
        }

        [HttpPost]
        [Route("AddItem")]
        public async Task<IActionResult> AddItemAsync(Item item)
        {            
            await _fusekiService.RegisterItem(item);
            return CreatedAtAction("AddItem", item);
            
        }

      


        



    }
}