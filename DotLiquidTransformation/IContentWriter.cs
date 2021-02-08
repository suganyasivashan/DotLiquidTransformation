using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotLiquidTransformation
{
    public interface IContentWriter
    {
        StringContent CreateResponse(string output);
    }
}
