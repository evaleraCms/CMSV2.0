using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Common.Extensions
{
    public static class TypeExtensions
    {
        public static ICollection<Type> GetClosedTypesFromType(this Type closedInterface, Assembly assembly = null)
        {
            assembly = assembly ?? closedInterface.Assembly;
            return assembly.GetTypes()
                .Where(t=>
                t.IsClass &&
                !t.IsAbstract &&
                t.GetInterfaces().Any(x=> !x.IsGenericType && x == closedInterface)).ToList();


        }


    }
}
