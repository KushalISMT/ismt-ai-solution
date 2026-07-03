using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Web.Controllers;

public class ArticlesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ArticlesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _context.Articles
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishDate)
            .ToListAsync();
        return View(articles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.ArticleId == id && a.IsPublished);

        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }
}
