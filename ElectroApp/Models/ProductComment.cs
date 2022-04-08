using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class ProductComment
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Forgot to write title of review")]
        [StringLength(maximumLength:40)]
        public string Title { get; set; }
        public DateTime WriteDate { get; set; }
        [Required(ErrorMessage = "Your forgot write your message")]
        [StringLength(maximumLength: 600, ErrorMessage = "You can include max 600 symbol")]
        public string Text { get; set; }
        public bool IsAccess { get; set; }
        public int ProductId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
