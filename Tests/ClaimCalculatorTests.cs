using Xunit;
using Claim = LecturerHourlyClaimApp.Models.Claim;

namespace LecturerHourlyClaimApp.Tests
{
    public class ClaimTests
    {
        // Test to ensure TotalClaim is calculated correctly
        [Fact]
        public void TotalClaim_ShouldReturnCorrectValue()
        {
            // Arrange: Create a fake claim with hours worked and hourly rate
            var fakeClaim = new Claim
            {
                HoursWorked = 8m,    // 8 hours worked
                HourlyRate = 25m     // Hourly rate of $25
            };

            // Act: Calculate the total claim
            var expectedTotal = 8m * 25m; // 200
            var actualTotal = fakeClaim.TotalClaim;

            // Assert: Check if the actual total matches the expected total
            Assert.Equal(expectedTotal, actualTotal);
        }

        // Test to ensure that 0 hours worked results in a 0 total claim
        [Fact]
        public void TotalClaim_ShouldReturnZeroWhenNoHoursWorked()
        {
            // Arrange: Create a fake claim with 0 hours worked
            var fakeClaim = new Claim
            {
                HoursWorked = 0m,    // 0 hours worked
                HourlyRate = 50m     // Hourly rate of $50
            };

            // Act: Calculate the total claim
            var expectedTotal = 0m;
            var actualTotal = fakeClaim.TotalClaim;

            // Assert: Check if the actual total is 0
            Assert.Equal(expectedTotal, actualTotal);
        }

        // Test to ensure TotalClaim works with fractional hours
        [Fact]
        public void TotalClaim_ShouldHandleDecimalHoursCorrectly()
        {
            // Arrange: Create a fake claim with fractional hours worked
            var fakeClaim = new Claim
            {
                HoursWorked = 3.5m,  // 3.5 hours worked
                HourlyRate = 40m     // Hourly rate of $40
            };

            // Act: Calculate the total claim
            var expectedTotal = 3.5m * 40m; // 140
            var actualTotal = fakeClaim.TotalClaim;

            // Assert: Check if the actual total matches the expected total
            Assert.Equal(expectedTotal, actualTotal);
        }
    }
}
