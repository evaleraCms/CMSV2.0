using CMS.Core.Extensions;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Api.Core.Filter
{
    public class SanitizeRequestFilter : Attribute, IAuthorizationFilter
    {
        private List<KeyValuePair<string, StringValues>> kvps;
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if(context.HttpContext.Request.Method.ToUpper() != "POST") return;

            var request = context.HttpContext.Request?.Headers;
            var contentType = request["Content-Type"].ToString();

            if (contentType.Contains("application/json"))
            {
                var requestbody = ReadRequestBodyAsync(context.HttpContext).Result;
                await WriteRequestBodyAsync(context.HttpContext, requestbody.Sanitized());
            }
            else if (contentType.Contains("multipart/form-data"))
            {
                try 
                { 
                    kvps = context.HttpContext.Request.Form.ToList();
                    var newForm = new Dictionary<string, StringValues>();
                    var sanitizer = new HtmlSanitizer();

                    foreach (var kvp in kvps) 
                    {
                        var key = kvp.Key;
                        var value = kvp.Value;
                        newForm.Add(key, sanitizer.Sanitize(value));
                    }
                    FormFileCollection formFiles = new FormFileCollection();
                    foreach (var item in context.HttpContext.Request.Form.Files)
                    {
                        if (!IsFileValid(item)) 
                        {
                            context.ModelState.AddModelError("Error", "Invalid file type");
                            return;                        
                        }
                        var newFormFile = new FormFile(item.OpenReadStream(), 0, item.Length,item.Name.Sanitized(),item.FileName.Sanitized());
                        formFiles.Add(newFormFile);
                    
                    }
                    context.HttpContext.Request.Form = new FormCollection(newForm, formFiles);
                }
                catch (Exception ex) 
                { 
                    context.ModelState.AddModelError("Error",ex.Message);
                
                }
            }
        }

        public static bool IsFileValid(IFormFile file)
        { 
            if(file == null || file.Length == 0)
                return false;

            var allowedSignatures = new Dictionary<string, List<byte[]>>
            {
                {".gif", new List<byte[]>{ new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
                {".png", new List<byte[]>{ new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
                {".jpg", new List<byte[]>{ new byte[] { 0xFF, 0xD8, 0xFF } } },
                {".jpeg", new List<byte[]>{ new byte[] { 0xFF, 0xD8, 0xFF } } },
                {".pdf", new List<byte[]>{ new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
                {".csv", new List<byte[]>{ new byte[] { 0xEF, 0xBB, 0xBF, },
                new byte[] { 0xFF,0xFE} ,
                new byte[] { 0xFE,0xFF} ,
                new byte[0] } } 

            };
        
            var ext = Path.GetExtension(file.Name).ToLowerInvariant();
            if(!allowedSignatures.ContainsKey(ext))
                return false;
            using(var reader = new BinaryReader(file.OpenReadStream()))
            {
                var maxHeaderSize = allowedSignatures[ext].Max(c=>c.Length);
                var headeerBytes = reader.ReadBytes(maxHeaderSize);

                //if (ext == ".csv")
                //{
                    
                //    return allowedSignatures[".csv"].Any(signature=> headeerBytes.Take(signature.Length).SequenceEqual(signature));
                //}

                return allowedSignatures[ext].Any(signature => headeerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
        private async Task WriteRequestBodyAsync(HttpContext context, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            await context.Response.Body.WriteAsync(data,0,data.Length);
            context.Request.Body.Position = 0;
        
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true))
            { 
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position=0;
                return body;
            
            }
        
        }
    }
}
