using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class CompareVM
    {
        public List<CompareItemVM> CompareItems { get; set; }
        public double TotalPrice { get; set; }
        public int Count { get; set; }
    }
}
