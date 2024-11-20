using LecturerHourlyClaimApp.Data;
using LecturerHourlyClaimApp.Models;
using LecturerHourlyClaimApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using LecturerHourlyClaimApp.Services;//Stores the majority of the systems automation features
using Claim = LecturerHourlyClaimApp.Models.Claim;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace LecturerHourlyClaimApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly ClaimVerificationService _verificationService;

        public HomeController()
        {
            // Initialize the verification service
            _verificationService = new ClaimVerificationService();
        }

        // Hardcoded user credentials
        private readonly Dictionary<string, (string password, string role)> _users = new Dictionary<string, (string password, string role)>
        {
            { "lecturer", ("password123", "Lecturer") },
            { "admin", ("adminpass", "Admin") },
            { "academicManager", ("managerpass", "AcademicManager") }
        };

        private static List<LecturerHourlyClaimApp.Models.Claim> claims = new List<LecturerHourlyClaimApp.Models.Claim>
        {
            new LecturerHourlyClaimApp.Models.Claim { Id = 1, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(1), HoursWorked = 8, HourlyRate = 50, Notes = "Worked on project", PersonId = 1, Status = "Pending"},
            new LecturerHourlyClaimApp.Models.Claim { Id = 2, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(2), HoursWorked = 6, HourlyRate = 50, Notes = "Lectured two classes", PersonId = 1, Status = "Pending"},
            new LecturerHourlyClaimApp.Models.Claim { Id = 3, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(4), HoursWorked = 181, HourlyRate = 50, Notes = "Lectured a class", PersonId = 1, Status = "Pending"},
            new LecturerHourlyClaimApp.Models.Claim { Id = 4, StartDate = System.DateTime.Today, EndDate = System.DateTime.Today.AddDays(4), HoursWorked = 7, HourlyRate = 50, Notes = "Lectured two classes", PersonId = 2, Status = "Pending"}
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
                    else if (userInfo.Role == "AcademicManager")
                    {
                        return RedirectToAction("ManagerMenu");
                    }
                    else if (userInfo.Role == "HR")
                    {
                        return RedirectToAction("HRMenu");
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

        public IActionResult ManagerMenu()
        {
            return View();
        }

        public IActionResult HRMenu()
        {
            return View();
        }

        public IActionResult ManageLecturer()
        {             
            return View(persons);
        }

        [HttpGet]
        public IActionResult GetPerson(int id)
        {
            Console.WriteLine($"GetPerson called with ID: {id}");
            var person = persons.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return Json(person);
        }

        [HttpPost]
        public IActionResult UpdatePerson([FromBody] Person updatedPerson)
        {
            var person = persons.FirstOrDefault(p => p.Id == updatedPerson.Id);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            person.FirstName = updatedPerson.FirstName;
            person.LastName = updatedPerson.LastName;
            person.HourlyRate = updatedPerson.HourlyRate;

            return Ok("Lecturer details updated successfully.");
        }






        public IActionResult ViewAllClaims()
        {
            // Retrieve the user ID of the logged-in user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            // Log the user ID
            Console.WriteLine($"User ID: {userId}"); // Log the user ID

            // Get all claims in the system
            var allClaims = claims
                .Select(c => new TrackClaimViewModel
                {
                    Id = c.Id,
                    PersonId = c.PersonId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Notes = c.Notes,
                    Status = c.Status,
                    SupportingDocumentPath = c.SupportingDocumentPath,
                    AdminComment = c.AdminComment
                }).ToList();

            var totalPayout = allClaims
                .Where(c => c.Status == "Approved") // Filter to only approved claims
                .Sum(c => c.HoursWorked * c.HourlyRate); // Calculate the sum for approved claims

            // Log the number of claims found and the total payout
            Console.WriteLine($"Total Claims Found: {allClaims.Count}");
            Console.WriteLine($"Total Approved Claims Payout: {totalPayout}");

            // Pass the total payout to the view
            ViewBag.TotalPayout = totalPayout;

            // Return the view with all claims
            return View(allClaims);

        }

        public IActionResult ExportClaimsToPDF()
        {
            // Generate claims data
            var allClaims = claims
                .Select(c => new TrackClaimViewModel
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Notes = c.Notes,
                    Status = c.Status
                }).ToList();

            // Calculate total for approved claims
            decimal totalApproved = allClaims
                .Where(c => c.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                .Sum(c => c.HoursWorked * c.HourlyRate);

            // Create a memory stream for the PDF
            using (var stream = new MemoryStream())
            {
                // Initialize iTextSharp PDF document
                Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Add a title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("Claims Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                pdfDoc.Add(title);

                // Add date and time
                var dateTimeFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var dateTime = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateTimeFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 10f
                };
                pdfDoc.Add(dateTime);

                // Create a table
                PdfPTable table = new PdfPTable(7); // Updated to include Status column
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 2, 2, 2, 1, 3, 2 }); // Adjust widths

                // Add table headers
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Start Date", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("End Date", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Hours Worked", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Hourly Rate", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Notes", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                // Add rows to the table
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var claim in allClaims)
                {
                    table.AddCell(new PdfPCell(new Phrase(claim.Id.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.StartDate.ToShortDateString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.EndDate.ToShortDateString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.HoursWorked.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.HourlyRate.ToString("C"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Notes, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Status, cellFont)));
                }

                // Add the table to the document
                pdfDoc.Add(table);

                // Add total for approved claims
                var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var totalParagraph = new Paragraph($"Total for Approved Claims: {totalApproved:C}", totalFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20f
                };
                pdfDoc.Add(totalParagraph);

                // Close the document
                pdfDoc.Close();

                // Return the generated PDF as a file
                return File(stream.ToArray(), "application/pdf", "ClaimsReport.pdf");
            }
        }

        public IActionResult ExportApprovedClaimsToPDF()
        {
            // Generate claims data
            var allClaims = claims.Where(c => c.Status == "Approved")
                .Select(c => new TrackClaimViewModel
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Notes = c.Notes,
                    Status = c.Status
                }).ToList();

            // Calculate total for approved claims
            decimal totalApproved = allClaims
                .Where(c => c.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                .Sum(c => c.HoursWorked * c.HourlyRate);

            // Create a memory stream for the PDF
            using (var stream = new MemoryStream())
            {
                // Initialize iTextSharp PDF document
                Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Add a title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("Claims Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                pdfDoc.Add(title);

                // Add date and time
                var dateTimeFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var dateTime = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateTimeFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 10f
                };
                pdfDoc.Add(dateTime);

                // Create a table
                PdfPTable table = new PdfPTable(7); // Updated to include Status column
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 2, 2, 2, 1, 3, 2 }); // Adjust widths

                // Add table headers
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Start Date", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("End Date", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Hours Worked", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Hourly Rate", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Notes", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                // Add rows to the table
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var claim in allClaims)
                {
                    table.AddCell(new PdfPCell(new Phrase(claim.Id.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.StartDate.ToShortDateString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.EndDate.ToShortDateString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.HoursWorked.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.HourlyRate.ToString("C"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Notes, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Status, cellFont)));
                }

                // Add the table to the document
                pdfDoc.Add(table);

                // Add total for approved claims
                var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var totalParagraph = new Paragraph($"Total for Approved Claims: {totalApproved:C}", totalFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20f
                };
                pdfDoc.Add(totalParagraph);

                // Close the document
                pdfDoc.Close();

                // Return the generated PDF as a file
                return File(stream.ToArray(), "application/pdf", "ClaimsReport.pdf");
            }
        }


        public IActionResult ViewApprovedClaims()
        {
            // Retrieve the user ID of the logged-in user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            // Log the user ID
            Console.WriteLine($"User ID: {userId}"); // Log the user ID

            // Get all claims in the system
            var approvedClaims = claims.Where(c => c.Status == "Approved")
                .Select(c => new TrackClaimViewModel
                {
                    Id = c.Id,
                    PersonId = c.PersonId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    Notes = c.Notes,
                    Status = c.Status,
                    SupportingDocumentPath = c.SupportingDocumentPath,
                    AdminComment = c.AdminComment
                }).ToList();

            var totalPayout = approvedClaims
                .Where(c => c.Status == "Approved") // Filter to only approved claims
                .Sum(c => c.HoursWorked * c.HourlyRate); // Calculate the sum for approved claims

            // Log the number of claims found and the total payout
            Console.WriteLine($"Total Claims Found: {approvedClaims.Count}");
            Console.WriteLine($"Total Approved Claims Payout: {totalPayout}");

            // Pass the total payout to the view
            ViewBag.TotalPayout = totalPayout;

            // Return the view with all claims
            return View(approvedClaims);

        }

        public IActionResult PendingClaims()
        {
            var pendingClaims = claims.Where(c => c.Status == "Pending").ToList();//Will only retrieve claims with the pending status
            return View(pendingClaims);
        }

        // Verify a claim by ID (admin functionality)
        /*
        public IActionResult VerifyClaim(int id, string adminComment)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                var verificationResult = _verificationService.VerifyClaim(claim);

                if (!verificationResult.IsValid)
                {
                    TempData["Error"] = $"Claim #{id} cannot be verified: {verificationResult.Message}";
                }
                else
                {
                    claim.Status = "Approved";
                    claim.AdminComment = adminComment;
                    TempData["Message"] = $"Claim #{id} has been verified and approved.";
                }
            }
            return RedirectToAction("PendingClaims");
        }
        */

        public IActionResult ApproveClaimByAdmin(int id, string adminComment)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                var verificationResult = _verificationService.VerifyClaim(claim);

                if (!verificationResult.IsValid)
                {
                    TempData["Error"] = $"Claim #{id} cannot be verified: {verificationResult.Message}";
                }
                else
                {
                    claim.Status = "Pending Manager Approval";
                    claim.AdminComment = adminComment;
                    claim.IsAdminApproved = true;
                    TempData["Message"] = $"Claim #{id} has been verified and approved.";
                }
            }
            return RedirectToAction("PendingClaims");
        }


        public IActionResult PendingManagerClaims()
        {
            var pendingClaims = claims.Where(c => c.Status == "Pending Manager Approval").ToList();//Will only retrieve claims with the pending status
            return View(pendingClaims);
        }

        public IActionResult ApproveClaimByManager(int id, string adminComment)
        {
            var claim = claims.FirstOrDefault(c => c.Id == id && c.IsAdminApproved);
            if (claim != null)
            {
                claim.IsManagerApproved = true; // Manager approval
                claim.AdminComment = adminComment;
                claim.Status = "Approved"; // Final approval
                TempData["Message"] = $"Claim #{id} has been verified and approved.";
            }
            return RedirectToAction("PendingManagerClaims");
        }


        // Reject a claim by ID (admin functionality)
        public IActionResult RejectClaim(int id, string adminComment)
        {
            if (string.IsNullOrWhiteSpace(adminComment))
            {
                TempData["Error"] = "You must provide a note when rejecting a claim.";
                return RedirectToAction("PendingManagerClaims");
            }
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                claim.AdminComment = adminComment;
                TempData["Message"] = $"Claim #{id} has been rejected.";
            }
            return RedirectToAction("PendingManagerClaims");
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
                    Status = c.Status, 
                    SupportingDocumentPath = c.SupportingDocumentPath,
                    AdminComment = c.AdminComment
                }).ToList();

            var totalPayout = lecturerClaims
                .Where(c => c.Status == "Approved") // Filter to only approved claims
                .Sum(c => c.HoursWorked * c.HourlyRate); // Calculate the sum for approved claims

                       // Log the number of claims found and the total payout
                   Console.WriteLine($"Lecturer Claims Found: {lecturerClaims.Count}");
                   Console.WriteLine($"Total Approved Claims Payout: {totalPayout}");

            // Pass the total payout to the view
            ViewBag.TotalPayout = totalPayout;

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

                // Create a new claim object based on the model input
                var newClaim = new Claim
                {
                    Id = claims.Count + 1, // Simple ID assignment
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes,
                    PersonId = 1, // Set to a valid PersonId as needed
                    IsAdminApproved = false,
                    IsManagerApproved = false,
                    Status = "Pending"
                };

                // Verify the claim using ClaimVerificationService
                var verificationResult = _verificationService.VerifyClaim(newClaim);

                if (!verificationResult.IsValid)
                {
                    // Add verification error message to ModelState if claim is invalid
                    ModelState.AddModelError("", verificationResult.Message);
                    return View(model);
                }

                // Add supporting document logic if verified
                if (model.SupportingDocument != null && model.SupportingDocument.Length > 0)
                {
                    if (!model.SupportingDocument.ContentType.Equals("application/pdf"))
                    {
                        ModelState.AddModelError("SupportingDocument", "Please upload a valid PDF document.");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents");
                    var filePath = Path.Combine(uploadsFolder, model.SupportingDocument.FileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.SupportingDocument.CopyTo(stream);
                    }

                    newClaim.SupportingDocumentPath = $"/Documents/{model.SupportingDocument.FileName}";
                }

                // Add claim to the list after passing verification
                claims.Add(newClaim);

                ViewBag.Message = "Your claim has been successfully submitted!";
                ViewBag.TotalClaim = newClaim.TotalClaim.ToString("C");
                return View(model);
            }

            return View(model);
        }
    }
}
