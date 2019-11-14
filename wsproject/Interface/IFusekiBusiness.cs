using ApiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiRefibra.Interface
{
    public interface IFusekiBusiness
    {

        List<string> GetDataSetNames();
        List<RDFModel> RegisterItem(ItemRefibraModel item, string dataSet);
        IEnumerable<Object> GetAllItens(string dataSet);
        Object GetItemByName(string item, string dataSet);
        IEnumerable<Object> GetAllItensRelation(string dataSet);
        IEnumerable<Object> GetAllRelationsNames(string dataSet);
        IEnumerable<Object> GetItensByRelationName(string relationName, string dataSet);
    }
}
