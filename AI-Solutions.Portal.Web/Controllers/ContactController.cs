using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;
using AI_Solutions.Portal.Web.Models;
using AI_Solutions.Portal.Web.Models.Entities;
using AI_Solutions.Portal.Web.Models.ViewModels;

namespace AI_Solutions.Portal.Web.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.Countries = Countries.All
            .Select(c => new SelectListItem { Value = c, Text = c })
            .ToList();

        ViewBag.JobCategories = JobCategories.All
            .Select(c => new SelectListItem { Value = c, Text = c })
            .ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = Countries.All
                .Select(c => new SelectListItem { Value = c, Text = c })
                .ToList();

            ViewBag.JobCategories = JobCategories.All
                .Select(c => new SelectListItem { Value = c, Text = c })
                .ToList();

            return View("Index", model);
        }

        // Check if customer already exists by email
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == model.Email);

        if (customer == null)
        {
            customer = new Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                CompanyName = model.CompanyName,
                Country = model.Country
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        var inquiry = new Inquiry
        {
            CustomerId = customer.CustomerId,
            JobTitle = model.JobTitle,
            JobCategory = model.JobCategory,
            JobDetails = model.JobDetails,
            Status = "New"
        };

        _context.Inquiries.Add(inquiry);
        await _context.SaveChangesAsync();

        return RedirectToAction("ThankYou");
    }

    public IActionResult ThankYou()
    {
        return View();
    }
}
