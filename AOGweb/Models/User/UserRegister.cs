using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models.User
{
    public class UserRegister
    {
        [Required]
        [StringLength(50, ErrorMessage = "Device name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Device name cannot be longer than 50 characters.")]
        [RegularExpression(@"^\w+$", ErrorMessage = "Only letters and numbers are allowed.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Password cannot be less than 6 and longer than 50 characters.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1)]
        [Compare(nameof(Password), ErrorMessage ="Make sure both passwords are the same")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
