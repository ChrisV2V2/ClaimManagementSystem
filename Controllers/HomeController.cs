using LecturerHourlyClaimApp.Data;
using LecturerHourlyClaimApp.Models;
using LecturerHourlyClaimApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Claim = LecturerHourlyClaimApp.Models.Claim;

namespace LecturerHourlyClaimApp.Controllers
{
    public class HomeController : Controller
    {
        // Hardcoded user credentials
        private readonly Dictionary<string, (string password, string role)> _users = new Dictionary<string, (string password, string role)>
        {
            { "lecturer", ("password123", "Lecturer") },
            { "admin", ("adminpass", "Admin") }
        };

        private static List<LecturerHourlyClaimApp.Models.Claim> claims = new List<LecturerHourlyClaimApp.Models.Claim>
        {
            new LecturerHourlyClaimApp.Models.Claim { Id = 1, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(1), HoursWorked = 8, HourlyRate = 50, Notes = "Worked on project", PersonId = 1, Status = "Pending" },
            new LecturerHourlyClaimApp.Models.Claim { Id = 2, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(2), HoursWorked = 6, HourlyRate = 50, Notes = "Lectured two classes", PersonId = 1, Status = "Pending"}
        };

        private static List<Person> persons = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", HourlyRate = 50 },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", HourlyRate = 60 }
        };

        // Display the login page
        public IActionResult Index()
        {
            return View();
        }

        // Handle login submission
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                LecturerDbContext db = new LecturerDbContext();
                db.Database.EnsureCreated();
                var userInfo = db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
                if (userInfo != null)
                {
                    // Redirect based on the role
                    if (userInfo.Role == "Lecturer")
                    {
                        return RedirectToAction("LecturerMenu");
                    }
                    else if (userInfo.Role == "Admin")
                    {
                        return RedirectToAction("AdminMenu");
                    }
                }

                // Invalid credentials
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // Return the login view with the error message
            return View("Index", model);
        }

        // Lecturer Main Menu
        public IActionResult LecturerMenu()
        {
            return View();
        }

        // Admin Main Menu
        public IActionResult AdminMenu()
        {
            return View();
        }

        public IActionResult PendingClaims()
        {
            var pendingClaims = claims; // Retrieve claims for display
            return View(pendingClaims);
        }

        // Verify a claim by ID (admin functionality)
        public IActionResult VerifyClaim(int id, string adminComment)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = "Approved";
                claim.AdminComment = adminComment; // Set the admin comment
                TempData["Message"] = $"Claim #{id} has been verified.";
            }
            return RedirectToAction("PendingClaims");
        }

        // Reject a claim by ID (admin functionality)
        public IActionResult RejectClaim(int id, string adminComment)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                claim.AdminComment = adminComment; // Set the admin comment
                TempData["Message"] = $"Claim #{id} has been rejected.";
            }
            return RedirectToAction("PendingClaims");
        }

        public IActionResult SubmitClaim()
        {
            var model = new SubmitClaimViewModel
            {
                HourlyRate = 50m // Hardcoded hourly rate
            };
            return View(model);
        }

        private string GetClaimStatus(Claim claim)
        {
            return claim.Status;
        }

        public IActionResult TrackClaims()
        {
            // Retrieve the user ID of the logged-in user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            // Log the user ID
            Console.WriteLine($"User ID: {userId}"); // Log the user ID

            // Get claims submitted by the lecturer using the user ID or by ID 1
            var lecturerClaims = claims.Where(c => c.PersonId == userId || c.PersonId == 1)
                .Select(c => new TrackClaimViewModel
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Notes = c.Notes,
                    Status = GetClaimStatus(c), // Use the helper method to determine status
                    SupportingDocumentPath = c.SupportingDocumentPath,
                    AdminComment = c.AdminComment
                }).ToList();

            // Log number of claims found
            Console.WriteLine($"Lecturer Claims Found: {lecturerClaims.Count}");

            return View(lecturerClaims);
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index"); // Redirects to the login page
        }

        // Handle the Submit Claim form submission
        [HttpPost]
        public IActionResult SubmitClaim(SubmitClaimViewModel model)
        {
            model.HourlyRate = 50m; // Hardcoded hourly rate

            if (ModelState.IsValid)
            {
                if (model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("", "End date must be after the start date.");
                    return View(model);
                }

                if (model.HoursWorked <= 0)
                {
                    ModelState.AddModelError("HoursWorked", "Hours worked must be greater than zero.");
                    return View(model);
                }

                // Check if a file has been uploaded
                if (model.SupportingDocument != null && model.SupportingDocument.Length > 0)
                {
                    if (!model.SupportingDocument.ContentType.Equals("application/pdf"))
                    {
                        ModelState.AddModelError("SupportingDocument", "Please upload a valid PDF document.");
                        return View(model);
                    }

                    // Set the path where you want to save the uploaded file
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents");
                    var filePath = Path.Combine(uploadsFolder, model.SupportingDocument.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.SupportingDocument.CopyTo(stream);
                    }

                    // Assign the relative path to the claim
                    model.SupportingDocumentPath = $"/Documents/{model.SupportingDocument.FileName}";
                }

                // Create a new claim from the submitted data
                var newClaim = new Claim
                {
                    Id = claims.Count + 1, // Simple ID assignment
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes,
                    SupportingDocumentPath = model.SupportingDocumentPath, // Assign the document path
                    PersonId = 1, // Set to a valid PersonId as needed
                    Status = "Pending"
                };

                // Add the claim to the list
                claims.Add(newClaim);

                ViewBag.Message = "Your claim has been successfully submitted!";
                ViewBag.TotalClaim = newClaim.TotalClaim.ToString("C");
                return View(model);
            }

            return View(model);
        }
    }
}
