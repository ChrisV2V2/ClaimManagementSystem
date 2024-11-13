using System.ComponentModel.DataAnnotations;

namespace LecturerHourlyClaimApp.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Hours worked are required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Hours worked must be greater than zero.")]
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string? SupportingDocumentPath { get; set; }

        // Foreign key to link claim with a person
        public int PersonId { get; set; }

        // Navigation property to the Person
        public virtual Person Person { get; set; }
        public decimal TotalClaim => HoursWorked * HourlyRate;

        public string AdminComment { get; set; }

        public bool IsAdminApproved { get; set; } // Step 1 approval by Admin
        public bool IsManagerApproved { get; set; } // Step 2 approval by Academic Manager

        public string Status { get; set; }
    }
}
