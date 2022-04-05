using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please include product name")]
        [StringLength(maximumLength:50)]
        public string Name { get; set; }
        [Required(ErrorMessage ="Please include product price")]
        public double Price { get; set; }
        [Required(ErrorMessage ="Please include cost price")]
        public double CostPrice { get; set; }
        [Required(ErrorMessage ="Please include SKU code")]
        [StringLength(maximumLength:25)]
        public string SkuCode { get; set; }
        [StringLength(maximumLength:200)]
        public string Videolink { get; set; }
        [Required(ErrorMessage ="Please include product description")]
        [StringLength(maximumLength:600)]
        public string Description { get; set; }
        public bool InStock { get; set; }
        public int  AvaliableCount { get; set; }
        public Campaign Campaign { get; set; }
        public int? CampaignId { get; set; }
        public Specs Specs { get; set; }
        public int? SpecsId { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        [NotMapped]
        public List<int> CategoryIds { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        [NotMapped]
        public List<int> ImageIds { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }
    }
}
