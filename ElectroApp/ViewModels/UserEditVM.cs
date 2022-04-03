using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.ViewModels
{
    public class UserEditVM
    {
        [StringLength(maximumLength: 50)]
        public string Username { get; set; }
        [StringLength(maximumLength: 50)]
        public string Fullname { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ComfirmPassword { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
