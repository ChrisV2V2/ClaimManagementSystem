using System.ComponentModel.DataAnnotations;

namespace LecturerHourlyClaimApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // Remember to hash passwords in production

        public string Role { get; set; } // This could be "Lecturer", "Admin", etc.
    }
}
