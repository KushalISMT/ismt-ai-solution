using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Web.Controllers;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Upcoming()
    {
        var events = await _context.Events
            .Where(e => e.EventType == "Upcoming")
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        return View(events);
    }

    public async Task<IActionResult> Gallery()
    {
        var events = await _context.Events
            .Where(e => e.EventType == "Past")
            .Include(e => e.EventImages)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
        return View(events);
    }
}
