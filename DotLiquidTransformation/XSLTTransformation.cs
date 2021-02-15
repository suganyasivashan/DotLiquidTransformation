using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Net;
using System.Linq;
using DotLiquid;
using System.Text;

namespace DotLiquidTransformation
{
    public static class XSLTTransformation
    {
        [FunctionName("XSLTTransformation")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [Blob("xslt-transforms/samplexslt.xslt", FileAccess.Read)] Stream inputblob, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (inputblob == null)
            {
                log.LogError("inputBlog null");
                return req.CreateErrorResponse(HttpStatusCode.NotFound, "XSLT transform not found");
            }
            string requestContentType = req.Content.Headers.ContentType.MediaType;
            string responseContentType = req.Headers.Accept.FirstOrDefault().MediaType;

            var sr = new StreamReader(inputblob);
            var xsltTransform = sr.ReadToEnd();

            var contentReader = ContentFactory.GetContentReader(requestContentType);
            var contentWriter = ContentFactory.GetContentWriter(responseContentType);

            Hash inputHash;
            try
            {
                inputHash = await contentReader.ParseRequestAsync(req.Content);

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error parsing request body", ex);
            }

            // Register the Liquid custom filter extensions
            Template.RegisterFilter(typeof(CustomFilters));

            // Execute the Liquid transform
            Template template;

            try
            {
                template = Template.Parse(xsltTransform);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error parsing XSLT template", ex);
            }

            string output = string.Empty;

            try
            {
                output = template.Render(inputHash);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error rendering XSLT template", ex);
            }

            if (template.Errors != null && template.Errors.Count > 0)
            {
                if (template.Errors[0].InnerException != null)
                {
                    return req.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error rendering XSLT template: {template.Errors[0].Message}", template.Errors[0].InnerException);
                }
                else
                {
                    return req.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error rendering XSLT template: {template.Errors[0].Message}");
                }
            }

            try
            {
                var content = contentWriter.CreateResponse(output);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                // Just log the error, and return the Liquid output without parsing
                log.LogError(ex.Message, ex);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(output, Encoding.UTF8, responseContentType)
                };
            }
        }
    }
}
