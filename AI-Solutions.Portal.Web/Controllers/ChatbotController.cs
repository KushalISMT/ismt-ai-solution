using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Data;
using AI_Solutions.Portal.Web.Models.ViewModels;

namespace AI_Solutions.Portal.Web.Controllers;

public class ChatbotController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChatbotController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Server-side part of the client-hybrid chatbot.
    /// Handles dynamic, data-driven intents and provides fallback responses.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Process([FromBody] ChatbotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
        {
            return Json(new ChatbotResponse
            {
                Reply = "I didn't catch that. Could you rephrase?",
                Suggestions = CommonSuggestions()
            });
        }

        var input = request.Message.Trim();

        if (ContainsAny(input, "solution", "service", "product", "offer", "what do you do"))
        {
            var solutions = await _context.Solutions
                .Where(s => !string.IsNullOrEmpty(s.Title))
                .OrderBy(s => s.Title)
                .Take(5)
                .Select(s => s.Title)
                .ToListAsync();

            var reply = solutions.Any()
                ? $"Here are some of our AI solutions:<ul class='mb-0 mt-1'>{string.Join("", solutions.Select(s => $"<li>{Escape(s)}</li>"))}</ul>Would you like to know more about a specific one?"
                : "We offer a range of AI-powered solutions. Visit our Solutions page to learn more.";

            return Json(new ChatbotResponse
            {
                Reply = reply,
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "View Solutions", Type = "link", Value = "/Solutions" },
                    new() { Label = "Contact Us", Type = "link", Value = "/Contact" }
                }
            });
        }

        if (ContainsAny(input, "case study", "case studies", "client", "work", "portfolio"))
        {
            var caseStudies = await _context.CaseStudies
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .Select(c => new { c.Title, c.ClientName })
                .ToListAsync();

            var reply = caseStudies.Any()
                ? $"Here are some recent case studies:<ul class='mb-0 mt-1'>{string.Join("", caseStudies.Select(c => $"<li><strong>{Escape(c.Title)}</strong> — {Escape(c.ClientName)}</li>"))}</ul>"
                : "We have a growing portfolio of case studies. Visit the Case Studies page to read more.";

            return Json(new ChatbotResponse
            {
                Reply = reply,
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "Case Studies", Type = "link", Value = "/CaseStudies" },
                    new() { Label = "Contact Us", Type = "link", Value = "/Contact" }
                }
            });
        }

        if (ContainsAny(input, "article", "blog", "news", "read"))
        {
            var articles = await _context.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishDate)
                .Take(5)
                .Select(a => new { a.Title, a.Author })
                .ToListAsync();

            var reply = articles.Any()
                ? $"Latest articles:<ul class='mb-0 mt-1'>{string.Join("", articles.Select(a => $"<li><strong>{Escape(a.Title)}</strong> by {Escape(a.Author)}</li>"))}</ul>"
                : "Check out our Articles page for the latest insights.";

            return Json(new ChatbotResponse
            {
                Reply = reply,
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "Articles", Type = "link", Value = "/Articles" }
                }
            });
        }

        if (ContainsAny(input, "event", "webinar", "upcoming", "calendar"))
        {
            var events = await _context.Events
                .Where(e => e.EventType == "Upcoming")
                .OrderBy(e => e.EventDate)
                .Take(5)
                .Select(e => new { e.Title, e.EventDate, e.Location })
                .ToListAsync();

            var reply = events.Any()
                ? $"Upcoming events:<ul class='mb-0 mt-1'>{string.Join("", events.Select(e => $"<li><strong>{Escape(e.Title)}</strong> — {e.EventDate:yyyy-MM-dd} in {Escape(e.Location)}</li>"))}</ul>"
                : "We don't have any upcoming events listed right now. Visit the Events page for the gallery.";

            return Json(new ChatbotResponse
            {
                Reply = reply,
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "Upcoming Events", Type = "link", Value = "/Events/Upcoming" },
                    new() { Label = "Event Gallery", Type = "link", Value = "/Events/Gallery" }
                }
            });
        }

        if (ContainsAny(input, "contact", "email", "inquiry", "quote", "reach", "get in touch"))
        {
            return Json(new ChatbotResponse
            {
                Reply = "You can reach us through the Contact Us form. We'll get back to you quickly.",
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "Contact Us", Type = "link", Value = "/Contact" }
                }
            });
        }

        if (ContainsAny(input, "feedback", "review", "testimonial"))
        {
            return Json(new ChatbotResponse
            {
                Reply = "We value your feedback. You can view testimonials or submit your own.",
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "View Feedback", Type = "link", Value = "/Feedback" },
                    new() { Label = "Submit Feedback", Type = "link", Value = "/Feedback/Submit" }
                }
            });
        }

        if (ContainsAny(input, "admin", "login", "dashboard", "back office"))
        {
            return Json(new ChatbotResponse
            {
                Reply = "The admin area is password-protected. Please use the Login link to access it.",
                Suggestions = new List<ChatbotSuggestion>
                {
                    new() { Label = "Login", Type = "link", Value = "/Identity/Account/Login" }
                }
            });
        }

        return Json(new ChatbotResponse
        {
            Reply = "I'm not sure I understand. I can help with solutions, case studies, articles, events, feedback, or contact information.",
            Suggestions = CommonSuggestions()
        });
    }

    private static List<ChatbotSuggestion> CommonSuggestions()
    {
        return new List<ChatbotSuggestion>
        {
            new() { Label = "Solutions", Type = "quick", Value = "What solutions do you offer?" },
            new() { Label = "Case Studies", Type = "quick", Value = "Show me case studies" },
            new() { Label = "Contact Us", Type = "link", Value = "/Contact" }
        };
    }

    private static bool ContainsAny(string input, params string[] terms)
    {
        return terms.Any(t => input.Contains(t, StringComparison.InvariantCultureIgnoreCase));
    }

    private static string Escape(string text)
    {
        return System.Net.WebUtility.HtmlEncode(text);
    }
}
