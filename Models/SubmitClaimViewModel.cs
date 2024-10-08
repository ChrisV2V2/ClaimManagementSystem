using System.ComponentModel.DataAnnotations;

namespace LecturerHourlyClaimApp.Models
{
    public class SubmitClaimViewModel
    {
        [Required]
        [Range(0, 1000, ErrorMessage = "Please enter a valid number of hours.")]
        public int HoursWorked { get; set; }

        public decimal HourlyRate { get; set; } = 50m;//The system will set the lecturer's hourly rate 

        // Notes field to allow the lecturer to add extra information
        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today; // Default to today's date

        // End date of the claim period
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);//End date is one day after the current date

        // This property will calculate the total claim
        public decimal TotalClaim => HoursWorked * HourlyRate;
    }
}
