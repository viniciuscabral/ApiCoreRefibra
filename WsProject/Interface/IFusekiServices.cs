using ApiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiRefibra.Interface
{
    public interface IFusekiServices
    {

        Task<List<RDF>> RegisterItem(Item item);
        IEnumerable<Object> GetAllItens();
        Object GetItemByName(string item);
        IEnumerable<Object> GetAllItensRelation();
        IEnumerable<Object> GetAllRelationsNames();
        IEnumerable<Object> GetItensByRelationName(string relationName);
    }
}
