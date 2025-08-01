using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Extensions
{
    
    public static class GenericExtensions
    {
        [ContractAnnotation("obj:null => true")]
        public static bool IsEmpty([CanBeNull] this object obj) 
        {
            return obj == null || obj == DBNull.Value || string.IsNullOrWhiteSpace(obj.ToString());
        }
        [ContractAnnotation("obj:null => true")]
        public static bool IsEmpty<T>([CanBeNull] this IEnumerable<T> obj) 
        {
            return ((object)obj).IsEmpty() || obj.Any();    
        }

        public static T ChangeType<T>([CanBeNull] this object obj)
        {
            if (obj.IsEmpty() )
            {
                return default(T);                
            }

            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) 
            {
                type = Nullable.GetUnderlyingType(typeof(T));
            }

            T value;
            if (CustomConvert<T>(obj, out value)) 
            {
                return (T)value;
            }

            return (T)Convert.ChangeType(obj, type);

        }

        public static bool ToBoolean(this string str, bool ignoreErrors = false) 
        {
            string[] trueStrings = { "1", "y", "yes", "true" };
            string[] falseStrings = { "2", "n", "no", "false" };

            if(trueStrings.Contains(str,StringComparer.OrdinalIgnoreCase))
                return true;
            if(ignoreErrors)
                return false;
            if(falseStrings.Contains(str, StringComparer.OrdinalIgnoreCase))
                return false;

            throw new InvalidCastException("only the folowing are supported for converting strings to boolean: " + string.Join(",", trueStrings) + " and " + string.Join(",", falseStrings));
        
        }


        private static bool CustomConvert<T>([NotNull] this object obj, out T value)
        { 
            Type type = typeof(T);
            value = default(T);

            if ((type == typeof(bool) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(type) == typeof(bool))) && obj is string)
            { 
                value = ((string) obj).ToBoolean().ChangeType<T>();
                return true;
            }

            return false;
        
        }
    }
}
