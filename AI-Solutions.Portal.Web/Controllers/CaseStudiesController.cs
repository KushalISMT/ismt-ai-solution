using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Web.Controllers;

public class CaseStudiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CaseStudiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var caseStudies = await _context.CaseStudies
            .Include(c => c.Solution)
            .ToListAsync();
        return View(caseStudies);
    }
}
