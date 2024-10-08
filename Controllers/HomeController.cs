using LecturerHourlyClaimApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                    if (_users.TryGetValue(model.Username, out var userInfo) && userInfo.password == model.Password)
                    {
                        // Redirect based on the role
                        if (userInfo.role == "Lecturer")
                        {
                            return RedirectToAction("LecturerMenu");
                        }
                        else if (userInfo.role == "Admin")
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

        public IActionResult SubmitClaim()
        {
            var model = new SubmitClaimViewModel
            {
                HourlyRate = 50m // Hardcoded hourly rate
            };
            return View(model);
        }

        // Handle the Submit Claim form submission
        [HttpPost]
        public IActionResult SubmitClaim(SubmitClaimViewModel model)
        {
            model.HourlyRate = 50m; // Hardcoded hourly rate

            if (ModelState.IsValid)
            {
                // Ensure end date is after start date
                if (model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("", "End date must be after the start date.");
                    return View(model); // Return the model with validation errors
                }

                // Successful submission
                ViewBag.Message = "Your claim has been successfully submitted!";
                ViewBag.TotalClaim = model.TotalClaim.ToString("C"); // Format for currency
            }

            // Return the model back to the view even if there are validation errors
            return View(model);
        }
    }
}
