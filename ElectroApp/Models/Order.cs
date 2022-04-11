using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Adress { get; set; }
        [Required]
        public string AdressSec { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public bool? Status { get; set; }
        public string Message { get; set; }
    }
}
