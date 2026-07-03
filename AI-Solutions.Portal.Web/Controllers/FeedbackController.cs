using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;
using AI_Solutions.Portal.Web.Models.Entities;
using AI_Solutions.Portal.Web.Models.ViewModels;

namespace AI_Solutions.Portal.Web.Controllers;

public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _context;

    public FeedbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var feedbacks = await _context.Feedbacks
            .Where(f => f.IsApproved)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        var averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
        ViewBag.AverageRating = averageRating;

        return View(feedbacks);
    }

    [HttpGet]
    public IActionResult Submit()
    {
        return View(new FeedbackFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(FeedbackFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var feedback = new Feedback
        {
            CustomerName = model.CustomerName,
            CompanyName = model.CompanyName,
            Rating = model.Rating,
            Comment = model.Comment,
            IsApproved = true // Visible immediately; admin can hide later
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        return RedirectToAction("ThankYou");
    }

    public IActionResult ThankYou()
    {
        return View();
    }
}
