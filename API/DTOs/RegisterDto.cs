using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,}$", ErrorMessage =$"Invalid Password! " + $"Password must be at least 8 characters long " + $"and contain at least one lowercase, one uppercase, one number and one special character.")]
        public string Password { get; set; }

        internal object ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}