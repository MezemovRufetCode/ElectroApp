using ElectroApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class AccountVM
    {
        public AppUser AppUser { get; set; }
        public string Token { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Please fill this field")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please fill this field")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirm password is not match,please check it again")]
        public string ConfirmPassword { get; set; }
    }
}
