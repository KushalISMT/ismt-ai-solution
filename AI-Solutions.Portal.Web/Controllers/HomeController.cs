using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;
using AI_Solutions.Portal.Web.Models;

namespace AI_Solutions.Portal.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredSolutions = await _context.Solutions
            .Where(s => s.IsFeatured)
            .OrderBy(s => s.Title)
            .Take(3)
            .ToListAsync();

        var latestArticles = await _context.Articles
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishDate)
            .Take(3)
            .ToListAsync();

        ViewBag.FeaturedSolutions = featuredSolutions;
        ViewBag.LatestArticles = latestArticles;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
