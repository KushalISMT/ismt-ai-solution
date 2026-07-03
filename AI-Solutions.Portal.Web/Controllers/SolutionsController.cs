using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Web.Controllers;

public class SolutionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SolutionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var solutions = await _context.Solutions
            .Include(s => s.CaseStudies)
            .ToListAsync();
        return View(solutions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var solution = await _context.Solutions
            .Include(s => s.CaseStudies)
            .FirstOrDefaultAsync(s => s.SolutionId == id);

        if (solution == null)
        {
            return NotFound();
        }

        return View(solution);
    }
}
