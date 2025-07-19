using CMS.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace CMS.Api.Core.Filter
{
    public class SanitizedResponseFilter : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var originalBodyStream = context.HttpContext.Request.Body;
            using (var memoryStream = new MemoryStream()) 
            { 
                context.HttpContext.Request.Body = memoryStream;
                await next();

                memoryStream.Position = 0;
                var resposeBody =await new StreamReader(memoryStream).ReadToEndAsync();

                var sanitizedBytes = Encoding.UTF8.GetBytes(resposeBody.Sanitized());
                context.HttpContext.Response.Body = originalBodyStream;
                context.HttpContext.Response.ContentLength = sanitizedBytes.Length;
                await context.HttpContext.Response.Body.WriteAsync(sanitizedBytes,0,sanitizedBytes.Length);
            }
        }
    }
}
