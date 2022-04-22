using ElectroApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class HomeVM
    {
        public List<IntroSlider> IntroSliders { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<Brand> Brands { get; set; }
        public List<ProductCategory> productCategories { get; set; }
        public Setting Settings { get; set; }
    }
}
