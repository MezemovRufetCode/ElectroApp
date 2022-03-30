using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Specs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
