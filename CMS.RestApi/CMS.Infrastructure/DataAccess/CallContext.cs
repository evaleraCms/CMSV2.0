using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        public static void LogicalSetData(string name, object data) => state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static object LogicalGetData(string name) => state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }

    public static class CallContext<T> 
    {
        static ConcurrentDictionary<string, AsyncLocal<T>> state = new ConcurrentDictionary<string, AsyncLocal<T>>();
        public static void LogicalSetData(string name, T data) => state.GetOrAdd(name, _ => new AsyncLocal<T>()).Value = data;

        public static object LogicalGetData(string name) => state.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : null;
    }
}
