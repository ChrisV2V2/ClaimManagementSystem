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
            return View();
        }

        // Handle the Submit Claim form submission
        [HttpPost]
        public IActionResult SubmitClaim(SubmitClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                // You can add claim processing logic here (e.g., saving the claim to a database)
                ViewBag.Message = "Your claim has been successfully submitted!";
            }

            return View(model);
        }
    }
}
