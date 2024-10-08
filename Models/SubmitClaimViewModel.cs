using System.ComponentModel.DataAnnotations;

namespace LecturerHourlyClaimApp.Models
{
    public class SubmitClaimViewModel
    {
        [Required]
        [Range(0, 1000, ErrorMessage = "Please enter a valid number of hours.")]
        public int HoursWorked { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Please enter a valid hourly rate.")]
        public decimal HourlyRate { get; set; }

        // This property will calculate the total claim
        public decimal TotalClaim => HoursWorked * HourlyRate;
    }
}
