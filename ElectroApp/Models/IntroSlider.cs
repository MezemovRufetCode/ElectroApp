using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class IntroSlider
    {
        public int Id { get; set; }
        public string BackImage { get; set; }
        [NotMapped]
        public IFormFile BackImageFile { get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        public string FirstDesc { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string SecDesc { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string SliderBannerImage { get; set; }
        [NotMapped]
        public IFormFile BannerImgFile { get; set; }
    }
}
