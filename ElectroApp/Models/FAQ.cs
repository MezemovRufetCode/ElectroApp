using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Models
{
    public class FAQ
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:150)]
        public string Question { get; set; }
        [Required]
        [StringLength(maximumLength:600)]
        public string Answer { get; set; }
    }
}
