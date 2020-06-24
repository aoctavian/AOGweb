using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models.User
{
    public class UserPassword
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Old password is not less than 6 and longer than 50 characters.")]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "New password cannot be less than 6 and longer than 50 characters.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1)]
        [Compare(nameof(NewPassword), ErrorMessage = "Make sure new passwords are the same")]
        [Display(Name = "New Confirm Password")]
        public string NewConfirmPassword { get; set; }
    }
}
