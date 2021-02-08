﻿using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotLiquidTransformation
{
    public interface IContentReader
    {
        Task<Hash> ParseRequestAsync(HttpContent content);
    }
}
