using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Please include username")]
        [StringLength(maximumLength:25)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please include fullname")]
        [StringLength(maximumLength: 50)]
        public string Fullname { get; set; }
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Email can not be empty,please fill it")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "You forgot to include password,please fill it")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Include password again")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Password and confirm password is not match,please check it again")]
        public string ConfirmPassword { get; set; }

        public bool Terms { get; set; }

    }
}
