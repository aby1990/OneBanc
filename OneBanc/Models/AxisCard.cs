using System;
using System.Collections.Generic;
using System.Text;

namespace OneBanc.Models
{
    class AxisCard : ICreditCard
    {
        public List<string[]> CardValues { get; set; }
        public int[] Mapping { get => new int[] { 0, 3, 1, 2 }; }
    }
}
