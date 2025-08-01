using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Enums
{
    public enum ValidationRule
    {
        Unknown = 0,
        ForbidWhitespace = 1,
        Required = 2,
        Remote  = 3,
        MinLength = 4,
        MaxLength = 5,
        Min = 6,
        Max = 7,
        MinDate = 8,
        MaxDate = 9,
        Range = 10,
        Step = 11,
        Email = 12,
        Url = 13,
        Date = 14,
        Deteiso = 15,
        Number = 16,
        Digits = 17,
        Unique = 18,
        Login = 19,
        Contains = 20

    }
}
