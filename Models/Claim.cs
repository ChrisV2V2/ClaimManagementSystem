namespace LecturerHourlyClaimApp.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }

        // Foreign key to link claim with a person
        public int PersonId { get; set; }

        // Navigation property to the Person
        public virtual Person Person { get; set; }
        public decimal TotalClaim => HoursWorked * HourlyRate;
    }
}
