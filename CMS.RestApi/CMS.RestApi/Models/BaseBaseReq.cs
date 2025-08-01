using CMS.Api.Models.Interfaces;
using System.Runtime.Serialization;

namespace CMS.Api.Models
{
    [DataContract(Namespace="http://cms.com/cmsapi/")]
    public abstract class BaseBaseReq<T>: IBaseRequest<T> where T : class, new()
    {     
        public abstract Guid Token { get; set; }
        public abstract T Request { get; set; } 
    }
    
}
