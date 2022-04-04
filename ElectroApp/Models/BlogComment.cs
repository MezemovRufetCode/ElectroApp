using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class BlogComment
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Your forgot write your message")]
        [StringLength(maximumLength:600,ErrorMessage ="You can include max 600 symbol")]
        public string Text { get; set; }
        public DateTime WriteTime { get; set; }
        public double Star { get; set; }
        public Blog Blog { get; set; }
        [Required]
        public int BlogId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
