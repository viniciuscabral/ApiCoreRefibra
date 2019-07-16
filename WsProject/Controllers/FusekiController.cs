using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiRefibra.Interface;
using ApiRefibra.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ApiRefibra.Controllers
{
    [Produces("application/json")]
    [ApiController]    
    [EnableCors("AllowAll")]

    public class FusekiController : Controller
    {
        private IFusekiServices _fusekiService;        
        public FusekiController(IFusekiServices fusekiService)
        {
            _fusekiService = fusekiService;
        }

        /// <summary>
        /// Get all database itens 
        /// </summary>
        /// <returns>Generic list with all itens in the database</returns>
        [HttpGet]
        [Route("GetAllItens")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]        
        public IActionResult GetAllItens() 
        {      
            return Ok(_fusekiService.GetAllItens());
        }

        /// <summary>
        /// Get a specific item by name
        /// </summary>
        /// <param name="itemName">Item name</param>
        /// <returns>Recovered itens</returns>
        [HttpGet]
        [Route("ItensByName")]
        public IActionResult ItensByName([FromQuery] String itemName)
        {
            return Ok(_fusekiService.GetItemByName(itemName));
        }

        /// <summary>
        /// Get all relations between itens
        /// </summary>
        /// <returns>Generic relational list </returns>
        [HttpGet]
        [Route("ItensRelation")]
        public IActionResult ItensRelation()
        {
            return Ok(_fusekiService.GetItensRelation());
        }

        /// <summary>
        /// Set a new item
        /// </summary>
        /// <param name="item">Item</param>
        /// <remarks>
        /// Sample Item:
        ///
        ///     POST /
        ///     {
        ///        "Name": "sampleItem",
        ///        "Text": "description item",
        ///        "Image": base64string [image]
        ///     }
        ///
        /// </remarks>
        /// <returns>A registred item</returns>
        [HttpPost]
        [Route("AddItem")]
        public async Task<IActionResult> AddItemAsync(Item item)
        {            
            List<RDF> List = await _fusekiService.RegisterItem(item);
            return CreatedAtAction("AddItemAsync", List);
            
        }

      


        



    }
}