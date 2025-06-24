using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username must not exceed 50 characters.")]
        public  string Username { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                 ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }  = string.Empty;

        [Required(ErrorMessage = "Password confirmation cannot be empty.")]
        [Compare("Password", ErrorMessage = "Password confirmation does not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(0[35789][0-9]{8})$",
            ErrorMessage = "Phone number is invalid. It must start with 03, 05, 07, 08, or 09 and be 10 digits long.")]
        public string Phone { get; set; } = string.Empty;
    }
}
