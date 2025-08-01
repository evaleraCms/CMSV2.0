using CMS.Core.Enums;
using System.Runtime.Serialization;

namespace CMS.Api.Models
{
    [DataContract(Namespace ="http://schemas.datacontract.org/2004/07/CmsContracts")]
    public class CMSStatus
    {
        public CMSStatus(): this(ResponseStatus.Fail)
        {
            
        }
        public CMSStatus(ResponseStatus status):this(status, string.Empty)
        {

        }
        public CMSStatus(ResponseStatus status, string description)
        {
            CMSStatusCode = status;
            StatusDescription = description;
        }
        [DataMember(Name = "CMSStatusCode")]
        public ResponseStatus CMSStatusCode { get; set; }

        [DataMember(Name = "StatusDescription")]
        public string StatusDescription { get; set; }

    }
}
