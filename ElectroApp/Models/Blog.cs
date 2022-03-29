using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Image { get; set; }
        [Required]
        [StringLength(maximumLength: 200)]
        public string Title { get; set; }
        [Required]
        public string Desc { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime PublishDate { get; set; }
        public string WrittenBy { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public List<BlogTag> BlogTags { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
    }
}
