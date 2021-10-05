using System;
using System.Collections.Generic;
using System.Text;

namespace OneBanc.Models
{
    class HdfcCard : ICreditCard
    {
        public List<string[]> CardValues { get; set; }
        public int[] Mapping { get => new int[] { 0, 1, 2, 2 }; }
    }
}
