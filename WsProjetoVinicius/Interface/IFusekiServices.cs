using ApiJenaFusekiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiJenaFusekiRefibra.Interface
{
    public interface IFusekiServices
    {

        Task<List<RDF>> RegisterItem(Item item);
        IEnumerable<Object> GetAllItens();
        Object GetItemByName(string item);
        IEnumerable<Object> GetItensRelation();

    }
}
