using CMS.Api.Models.Interfaces;
using CMS.Core.Enums;
using System.Runtime.Serialization;

namespace CMS.Api.Models
{
    [DataContract(Namespace = "http://cmsapi.com")]
    public class BaseRep<T>: IBaseResp<T> where T : class
    {
        [DataMember(Order = 3)]
        public string Massage { get; set; }
        [DataMember(Order = 2)]
        public ResponseStatus Status { get; set; }  

        [IgnoreDataMember]
        public CMSStatus CMSStatus { get; set; }
        [DataMember(Order = 1)]
        public T Result { get; set; }
       
    }
}
