using ApiJenaFusekiRefibra.Interface;
using ApiJenaFusekiRefibra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiJenaFusekiRefibra.Implementation
{
    public class Fuseki2 : IFusekiServices

    {
        public IEnumerable<string> GetAllItens()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetItensByName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetItensRelation()
        {
            throw new NotImplementedException();
        }

        public Task RegisterItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
