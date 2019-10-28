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
        private readonly IFusekiBusiness _fusekiBusiness;        
        public FusekiController(IFusekiBusiness fusekiService)
        {
            _fusekiBusiness = fusekiService;
        }

        /// <summary>
        /// Get all database names 
        /// </summary>
        /// <returns>Generic list with all database names</returns>
        [HttpGet]
        [Route("GetDataSetNames")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetDataSetNames()
        {
            return Ok(_fusekiBusiness.GetDataSetNames());
        }

        /// <summary>
        /// Get all database itens 
        /// </summary>
        /// <returns>Generic list with all itens in the database</returns>
        [HttpGet]
        [Route("GetAllItens")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]                
        public IActionResult GetAllItens(string dataSet) 
        {      
            return Ok(_fusekiBusiness.GetAllItens(dataSet));
        }

        /// <summary>
        /// Get a specific item by name
        /// </summary>
        /// <param name="itemName">Item name</param>
        /// <param name="dataSet">DataSet name</param>
        /// <returns>Recovered itens</returns>
        [HttpGet]
        [Route("ItensByName")]        
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]  
        [ProducesResponseType(500)]        
        public IActionResult ItensByName([FromQuery] String itemName, [FromQuery] string dataSet)
        {
            return Ok(_fusekiBusiness.GetItemByName(itemName, dataSet));
        }

        /// <summary>
        /// Get all relations between itens
        /// </summary>
        /// <returns>Generic relational list </returns>
        [HttpGet]
        [Route("ItensRelation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]  
        [ProducesResponseType(500)] 
         public IActionResult ItensRelation([FromQuery] string dataSet)
        {
            return Ok(_fusekiBusiness.GetAllItensRelation(dataSet));
        }

        /// <summary>
        /// Get all relations in database
        /// </summary>
        /// <returns>string list </returns>
        [HttpGet]
        [Route("GetAllRelationsNames")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]  
        [ProducesResponseType(500)] 
        public IActionResult GetAllRelationsNames([FromQuery] string dataSet)
        {
            return Ok(_fusekiBusiness.GetAllRelationsNames(dataSet));
        }

        /// <summary>
        /// Get itens by specific relation name
        /// </summary>
        /// <returns>itens with any relaton with the param name</returns>
        [HttpGet]
        [Route("GetItensByRelationName")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]  
        [ProducesResponseType(500)] 
        public IActionResult GetItensByRelationName([FromQuery] String relatioName, [FromQuery] string dataSet)
        {
            return Ok(_fusekiBusiness.GetItensByRelationName(relatioName, dataSet));
        }

        /// <summary>
        /// Set a new item
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="dataSet">DataSetName</param>
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]   
        [ProducesResponseType(500)]             
        [Route("AddItem")]
        public IActionResult AddItemAsync([FromBody]ItemRefibraModel item, [FromQuery] string dataSet)
        {            
            List<RDFModel> List = _fusekiBusiness.RegisterItem(item, dataSet);
            return CreatedAtAction("AddItemAsync", List);
            
        }
    }
}