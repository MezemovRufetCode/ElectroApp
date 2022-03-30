using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public int DiscountPercent { get; set; }
        public List<Product> Products { get; set; }
    }
}