using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Sanitized(this string value) 
        { 
            return new HtmlSanitizer().Sanitize(value);
        }
    }
}
