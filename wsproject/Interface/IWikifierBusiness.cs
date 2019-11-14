using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiRefibra.Model;

namespace ApiRefibra.Interface
{
    public interface IWikifierBusiness
    {
        WikifierObjModel ProcessarWikifier(string text);
    }
}
