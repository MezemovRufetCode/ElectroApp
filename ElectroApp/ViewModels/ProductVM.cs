using ElectroApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class ProductVM
    {
        public List<Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Category> Categories { get; set; }
    }
}
