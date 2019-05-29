using ApiJenaFusekiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiJenaFusekiRefibra.Interface
{
    public interface IFusekiServices
    {

        Task<List<RDF>> RegisterItem(Item item);
        IEnumerable<string> GetAllItens();
        IEnumerable<Object> GetItensByName();
        IEnumerable<Object> GetItensRelation();

    }
}
