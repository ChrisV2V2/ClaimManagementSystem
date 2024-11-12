using LecturerHourlyClaimApp.Models;

namespace LecturerHourlyClaimApp.Services
{
    public class ClaimVerificationService
    {
        private readonly decimal MaxAllowedHours = 180;//Maximum hours allowed to claim per month
                                                       //No individual can work more than 45 hours a week = 180 hours per month.
        private readonly decimal MinHourlyRate = 28;//Legal minimum hourly rate in south africa
        private readonly decimal MaxHourlyRate = 1500;

        public VerificationResult VerifyClaim(Claim claim)
        {
            if (claim.HoursWorked > MaxAllowedHours)
            {
                return new VerificationResult(false, "Exceeds maximum allowed hours.");
            }

            if (claim.HourlyRate < MinHourlyRate || claim.HourlyRate > MaxHourlyRate)
            {
                return new VerificationResult(false, "Hourly rate is outside the allowed range.");
            }

            return new VerificationResult(true, "Claim meets all criteria.");
        }
    }

    public class VerificationResult
    {
        public bool IsValid { get; }
        public string Message { get; }

        public VerificationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
