using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TransformationsTest
{
    public class IntegrationTest
    {
        [Fact]
        public async Task jsontojsonusingliquidtemp()
        {
            try
            {
                var name = "stella";
                
                var jsonSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }, Formatting = Formatting.Indented };

                var postData = JsonConvert.SerializeObject(name, jsonSettings);

                var url = "https://dotliquidtransformationsample.azurewebsites.net/api/liquidtransformer?code=dnSHAbg37t/af/wW/jsCmr6jRz9abYYAbsr1qLPPavXwnJbZx5d6Vw==&clientId=apim-DotLiquidTransformationSample-apim";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                var response = await httpClient.SendAsync(request);

                Assert.True(true);
            }
            catch (Exception ex)
            {
                var baseEx = ex.GetBaseException();
                throw;
            }
        }
    }
}
