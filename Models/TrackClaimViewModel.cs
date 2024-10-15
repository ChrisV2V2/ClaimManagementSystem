namespace LecturerHourlyClaimApp.ViewModels
{
    public class TrackClaimViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } // e.g., Pending, Verified, Rejected
        public string SupportingDocumentPath { get; set; }

        public decimal TotalClaim => HoursWorked * HourlyRate;

        public string AdminComment { get; set; }

    }
}
