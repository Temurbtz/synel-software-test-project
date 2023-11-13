using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynelTestProject.Models;
using System.Globalization;

namespace SynelTestProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TestProjectDbContext _context;
        public HomeController(ILogger<HomeController> logger,TestProjectDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.RowsProcessed = 0;
                return RedirectToAction("Index");
            }

            var employees = new List<Employee>();
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var payrollNumber = csv.GetField<string>(Utils.PayrollNumber);
                        var firstName = csv.GetField<string>(Utils.FirstName);
                        var surname = csv.GetField<string>(Utils.Surname);
                        var dateOfBirth = csv.GetField<string>(Utils.DateOfBirth);
                        var phoneNumber = csv.GetField<string>(Utils.PhoneNumber);
                        var mobileNumber = csv.GetField<string>(Utils.MobileNumber);
                        var address = csv.GetField<string>(Utils.Address);
                        var secondAddress = csv.GetField<string>(Utils.SecondAddress);
                        var postCode = csv.GetField<string>(Utils.PostCode);
                        var email = csv.GetField<string>(Utils.Email);
                        var startDate = csv.GetField<string>(Utils.StartDate);
                        if(string.IsNullOrEmpty(payrollNumber) ||
                           string.IsNullOrEmpty(firstName) ||
                           string.IsNullOrEmpty(surname) ||
                           string.IsNullOrEmpty(dateOfBirth) ||
                           string.IsNullOrEmpty(phoneNumber) ||
                           string.IsNullOrEmpty(mobileNumber) ||
                           string.IsNullOrEmpty(address) ||
                           string.IsNullOrEmpty(secondAddress) ||
                           string.IsNullOrEmpty(postCode) ||
                           string.IsNullOrEmpty(email) ||
                           string.IsNullOrEmpty(startDate) ||
                           string.IsNullOrEmpty(secondAddress))
                        {
                            continue;
                        }


                        var employee = new Employee
                        {
                            PayrollNumber = payrollNumber,
                            FirstName = firstName,
                            Surname = surname,
                            DateOfBirth = DateTime.ParseExact(dateOfBirth, "dd/M/yyyy",CultureInfo.InvariantCulture),
                            PhoneNumber = phoneNumber,
                            MobileNumber = mobileNumber,
                            Address = address,
                            SecondAddress = secondAddress,
                            PostCode = postCode,
                            Email = email,
                            StartDate = DateTime.ParseExact(startDate, "dd/M/yyyy",CultureInfo.InvariantCulture)
                        };
                        employees.Add(employee);
                    }
                        _context.Employees.AddRange(employees);
                        _context.SaveChanges();
                }

            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex, "Error parsing CSV file.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error saving data to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
            }
            ViewBag.RowsProcessed = employees.Count;
            List<Employee> sortedBySurname = employees.OrderBy(employee=>employee.Surname).ToList();
            return View(employees);
        }
        [HttpPost]
        [Route("update")]
        public IActionResult UpdateEmployee([FromBody] Employee updatedModel)
        {
            var existingModel = _context.Employees.Find(updatedModel.Id);
            if (existingModel != null)
            {
                _context.Entry(existingModel).CurrentValues.SetValues(updatedModel);
                _context.SaveChanges();
            }
            return Json(updatedModel);
        }
    }
}