using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string SkuCode { get; set; }
        public string Videolink { get; set; }
        public string Description { get; set; }
        public bool InStock { get; set; }
        public Campaign Campaign { get; set; }
        public int? CampaignId { get; set; }
        public Specs Specs { get; set; }
        public int SpecsId { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        [NotMapped]
        public List<int> CategoryIds { get; set; }
        [NotMapped]
        public IFormFile ImageFiles { get; set; }
        [NotMapped]
        public List<int> ImageIds { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }
    }
}
