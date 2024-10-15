using System.Security.Claims;

namespace LecturerHourlyClaimApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public decimal HourlyRate { get; set; }
        // Navigation property to link claims
        public virtual ICollection<Claim> Claims { get; set; }
    }
}
