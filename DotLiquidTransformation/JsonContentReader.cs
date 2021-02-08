﻿using DotLiquid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotLiquidTransformation
{
    public class JsonContentReader : IContentReader
    {
        public JsonContentReader()
        {

        }

        public async Task<Hash> ParseRequestAsync(HttpContent content)
        {
            string requestBody = await content.ReadAsStringAsync();

            var transformInput = new Dictionary<string, object>();

            var requestJson = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestBody, new DictionaryConverter());

            // Wrap the JSON input in another content node to provide compatibility with Logic Apps Liquid transformations
            transformInput.Add("content", requestJson);

            return Hash.FromDictionary(transformInput);
        }
    }
}
