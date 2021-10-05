using System;
using System.Collections.Generic;
using System.Text;

namespace OneBanc.Models
{
    class IdfcCard : ICreditCard
    {
        public List<string[]> CardValues { get; set; }
        public int[] Mapping { get => new int[] { 1, 0, 2, 2 }; }
    }
}
