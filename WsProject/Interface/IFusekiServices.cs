using ApiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiRefibra.Interface
{
    public interface IFusekiServices
    {

        List<string> GetDataSetNames();
        Task<List<RDF>> RegisterItem(Item item, string dataSet);
        IEnumerable<Object> GetAllItens(string dataSet);
        Object GetItemByName(string item, string dataSet);
        IEnumerable<Object> GetAllItensRelation(string dataSet);
        IEnumerable<Object> GetAllRelationsNames(string dataSet);
        IEnumerable<Object> GetItensByRelationName(string relationName, string dataSet);
    }
}
