using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Enums
{
    [DataContract(Name = "Status")]
    public enum ResponseStatus
    {
        [EnumMember(Value = "Unknon")]
        Default = 0,
        [EnumMember(Value = "Success")]
        Sussess = 1,
        [EnumMember(Value = "Fail")]
        Fail = 2,
        [EnumMember(Value = "Warning")]
        Warning = 3,
        [EnumMember(Value = "Error")]
        Error = 4,

    }
}
