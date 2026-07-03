using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;
using AI_Solutions.Portal.Web.Models;
using AI_Solutions.Portal.Web.Models.Entities;
using AI_Solutions.Portal.Web.Models.ViewModels;

namespace AI_Solutions.Portal.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public AdminController(ApplicationDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task<IActionResult> Index()
    {
        var inquiries = await _context.Inquiries.CountAsync();
        var customers = await _context.Customers.CountAsync();
        var feedbacks = await _context.Feedbacks.CountAsync();
        var averageRating = await _context.Feedbacks
            .Where(f => f.IsApproved)
            .AnyAsync() ? await _context.Feedbacks.Where(f => f.IsApproved).AverageAsync(f => (double?)f.Rating) ?? 0 : 0;

        var inquiriesByCountry = await _context.Customers
            .GroupBy(c => c.Country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .ToListAsync();

        var inquiriesByJobCategory = await _context.Inquiries
            .Where(i => !string.IsNullOrEmpty(i.JobCategory))
            .GroupBy(i => i.JobCategory)
            .Select(g => new { JobCategory = g.Key, Count = g.Count() })
            .ToListAsync();

        ViewBag.TotalInquiries = inquiries;
        ViewBag.TotalCustomers = customers;
        ViewBag.TotalFeedbacks = feedbacks;
        ViewBag.AverageRating = averageRating.ToString("0.0");
        ViewBag.InquiriesByCountry = inquiriesByCountry;
        ViewBag.InquiriesByJobCategory = inquiriesByJobCategory;

        return View();
    }

    public async Task<IActionResult> Inquiries()
    {
        var inquiries = await _context.Inquiries
            .Include(i => i.Customer)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();
        return View(inquiries);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var inquiry = await _context.Inquiries
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.InquiryId == id);

        if (inquiry == null)
        {
            return NotFound();
        }

        PopulateCountries(inquiry.Customer.Country);
        PopulateJobCategories(inquiry.JobCategory);

        var model = new InquiryEditViewModel
        {
            InquiryId = inquiry.InquiryId,
            CustomerId = inquiry.CustomerId,
            FullName = inquiry.Customer.FullName,
            Email = inquiry.Customer.Email,
            Phone = inquiry.Customer.Phone,
            CompanyName = inquiry.Customer.CompanyName,
            Country = inquiry.Customer.Country,
            JobTitle = inquiry.JobTitle,
            JobCategory = inquiry.JobCategory,
            JobDetails = inquiry.JobDetails,
            Status = inquiry.Status
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InquiryEditViewModel model)
    {
        if (id != model.InquiryId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            PopulateCountries(model.Country);
            PopulateJobCategories(model.JobCategory);
            return View(model);
        }

        var inquiry = await _context.Inquiries
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.InquiryId == id);

        if (inquiry == null)
        {
            return NotFound();
        }

        inquiry.JobTitle = model.JobTitle;
        inquiry.JobCategory = model.JobCategory;
        inquiry.JobDetails = model.JobDetails;
        inquiry.Status = model.Status;

        inquiry.Customer.FullName = model.FullName;
        inquiry.Customer.Email = model.Email;
        inquiry.Customer.Phone = model.Phone;
        inquiry.Customer.CompanyName = model.CompanyName;
        inquiry.Customer.Country = model.Country;

        _context.Update(inquiry);
        _context.Update(inquiry.Customer);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Inquiry updated successfully.";
        return RedirectToAction(nameof(Inquiries));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var inquiry = await _context.Inquiries
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.InquiryId == id);

        if (inquiry == null)
        {
            return NotFound();
        }

        _context.Inquiries.Remove(inquiry);

        // Optionally remove the customer if they have no other inquiries
        var otherInquiries = await _context.Inquiries
            .AnyAsync(i => i.CustomerId == inquiry.CustomerId && i.InquiryId != id);

        if (!otherInquiries)
        {
            _context.Customers.Remove(inquiry.Customer);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Inquiry deleted successfully.";
        return RedirectToAction(nameof(Inquiries));
    }

    [HttpGet]
    public async Task<IActionResult> SendEmail(int id)
    {
        var inquiry = await _context.Inquiries
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.InquiryId == id);

        if (inquiry == null)
        {
            return NotFound();
        }

        var model = new SendEmailViewModel
        {
            InquiryId = inquiry.InquiryId,
            ToEmail = inquiry.Customer.Email,
            ToName = inquiry.Customer.FullName,
            Subject = $"RE: Your inquiry about {inquiry.JobTitle}",
            Body = $"<p>Dear {inquiry.Customer.FullName},</p><p>Thank you for contacting AI-Solutions regarding <strong>{inquiry.JobTitle}</strong>.</p><p>We are reviewing your requirements and will get back to you shortly.</p><br><p>Best regards,</p><p><strong>AI-Solutions Team</strong></p>"
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmail(int id, SendEmailViewModel model)
    {
        if (id != model.InquiryId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _emailSender.SendEmailAsync(model.ToEmail, model.Subject, model.Body);
            TempData["SuccessMessage"] = $"Email sent successfully to {model.ToEmail}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to send email: {ex.Message}. Please check SMTP settings in appsettings.json.";
        }

        return RedirectToAction(nameof(Inquiries));
    }

    public async Task<IActionResult> ExportCsv()
    {
        var inquiries = await _context.Inquiries
            .Include(i => i.Customer)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("InquiryId,Date,FullName,Email,Phone,Company,Country,JobTitle,JobCategory,JobDetails,Status");

        foreach (var item in inquiries)
        {
            csv.AppendLine($"{item.InquiryId},{item.CreatedDate:yyyy-MM-dd},{EscapeCsv(item.Customer.FullName)},{item.Customer.Email},{item.Customer.Phone},{EscapeCsv(item.Customer.CompanyName)},{EscapeCsv(item.Customer.Country)},{EscapeCsv(item.JobTitle)},{EscapeCsv(item.JobCategory)},\"{item.JobDetails.Replace("\"", "\"\"")}\",{item.Status}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", "inquiries.csv");
    }

    public async Task<IActionResult> Feedback()
    {
        var feedbacks = await _context.Feedbacks
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
        return View(feedbacks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFeedbackVisibility(int id)
    {
        var feedback = await _context.Feedbacks.FindAsync(id);
        if (feedback == null) return NotFound();

        feedback.IsApproved = !feedback.IsApproved;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Feedback));
    }

    private void PopulateCountries(string? selectedCountry = null)
    {
        ViewBag.Countries = Countries.All
            .Select(c => new SelectListItem
            {
                Value = c,
                Text = c,
                Selected = c == selectedCountry
            })
            .ToList();
    }

    private void PopulateJobCategories(string? selectedCategory = null)
    {
        ViewBag.JobCategories = JobCategories.All
            .Select(c => new SelectListItem
            {
                Value = c,
                Text = c,
                Selected = c == selectedCategory
            })
            .ToList();
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
