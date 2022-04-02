using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        //[NotMapped]
        //public IFormFile LogoImgFile { get; set; }

        [Required]
        [StringLength(maximumLength:50)]
        [DataType(DataType.EmailAddress)]
        public string hEmail { get; set; }
        [Required]
        [StringLength(maximumLength:30)]
        public string hContact { get; set; }
    }
}
