using ElectroApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class OrderVM
    {
        [Required]
        [StringLength(maximumLength:50)]
        public string Fullname { get; set; }
        //[Required]
        //[StringLength(50)]
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        [StringLength(maximumLength:50)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Adress { get; set; }
        [Required]
        [StringLength(200)]
        public string AdressSec { get; set; }
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipPostal { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
