using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace rent.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter the user name")]
        [StringLength(17, ErrorMessage = "the password must be between 17 and 3", MinimumLength = 3)]

        public string Name { get; set; }
 
        [EmailAddress]
        [Required(ErrorMessage = "Please enter the Email")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [StringLength(20, ErrorMessage = "the password must be between 20 and 10", MinimumLength = 10)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "The Password must contains special character , numbers and capital letter ")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please enter the number")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "please enter a valid number")]
        public string Number { get; set; }
        [Required(ErrorMessage = "Please enter the city")]
        [StringLength(20, ErrorMessage = "the city must be between 20 and 10", MinimumLength = 3)]

        public string City { get; set; }
    }
}
