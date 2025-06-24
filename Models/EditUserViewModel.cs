using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class EditUserViewModel
    {
        
        public string FullName { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; } // Cho phép null
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }

}
