using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Feature
    {
        public int Id { get; set; }
        public string FeatureName { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
