using CMS.Infrastructure.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Common
{
    public static class ConfigSettings
    {

        //public static string CMSConnStr => AppData.Configuration["CMSConnString"];

        public static string CMSConnStr => GetAppSetting<string>("CMSConnString");

        private static T GetAppSetting<T>(string key, bool isRequired = true, object defaultValue = null)
        {
            var value = AppData.Configuration[key];
            if (!isRequired && value.IsEmpty())
            {
                if (defaultValue != null)
                { 
                    return (T)defaultValue;                
                }
                return default(T);
            
            }
            if (isRequired && value.IsEmpty())
                throw new ArgumentException($"config-appSetting[{key}] is missing.");

            return value.ChangeType<T>();

        }

    }

    
}
