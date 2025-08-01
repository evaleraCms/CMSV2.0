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
        public static bool EqualsIgnoreCase(this string value1, string value2) {

            return string.Compare(value1, value2, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
