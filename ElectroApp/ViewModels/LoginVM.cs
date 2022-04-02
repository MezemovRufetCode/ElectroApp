using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Forgto to include username,please fill it")]
        [StringLength(maximumLength:50)]
        public string Username { get; set; }
        [Required(ErrorMessage ="Forgot to icnlude password,please fill it")]
        [StringLength(maximumLength:20)]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
