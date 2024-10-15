using LecturerHourlyClaimApp.Data;
using LecturerHourlyClaimApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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

        private static List<LecturerHourlyClaimApp.Models.Claim> claims = new List<LecturerHourlyClaimApp.Models.Claim>//Not sure why this is refrencing a sys class??
        {
            new LecturerHourlyClaimApp.Models.Claim { Id = 1, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(1), HoursWorked = 8, HourlyRate = 50, Notes = "Worked on project", PersonId = 1 },
            new LecturerHourlyClaimApp.Models.Claim { Id = 2, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(2), HoursWorked = 6, HourlyRate = 50, Notes = "Lectured two classes", PersonId = 1 }
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
        public IActionResult VerifyClaim(int id)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                TempData["Message"] = $"Claim #{id} has been verified.";
                // Mark the claim as verified if needed (e.g., add a Verified property)
            }
            return RedirectToAction("PendingClaims");
        }

        // Reject a claim by ID (admin functionality)
        public IActionResult RejectClaim(int id)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claims.Remove(claim); // Simulating rejection by removing the claim
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

                // Create a new claim from the submitted data
                var newClaim = new LecturerHourlyClaimApp.Models.Claim
                {
                    Id = claims.Count + 1, // Simple ID assignment
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes,
                    PersonId = 1 // Set to a valid PersonId as needed
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
