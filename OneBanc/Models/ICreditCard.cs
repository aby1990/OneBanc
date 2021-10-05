using System;
using System.Collections.Generic;
using System.Text;

namespace OneBanc.Models
{
    interface ICreditCard
    {
        int[] Mapping { get; }

        List<string[]> CardValues { get; set; }
    }
}
