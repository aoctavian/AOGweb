using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models.User
{
    public class UserLogIn
    {
        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Device name cannot be less than 6 and longer than 50 characters.")]
        public string Password { get; set; }
    }
}
