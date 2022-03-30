using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:20)]
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
