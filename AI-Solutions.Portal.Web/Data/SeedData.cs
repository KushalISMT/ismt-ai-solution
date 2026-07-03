using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AI_Solutions.Portal.Web.Models.Entities;

namespace AI_Solutions.Portal.Web.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // Apply pending migrations
        await context.Database.MigrateAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Seed Admin Role
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Seed Admin User
        const string adminEmail = "admin@ai-solutions.com";
        const string adminPassword = "Admin@123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        // Seed Solutions
        if (!context.Solutions.Any())
        {
            context.Solutions.AddRange(
                new Solution
                {
                    Title = "AI-Powered Virtual Assistant",
                    Description = "An intelligent virtual assistant that responds to employee inquiries and automates routine tasks.",
                    Industry = "Enterprise IT",
                    Technologies = "ASP.NET Core, Azure AI, NLP",
                    IsFeatured = true
                },
                new Solution
                {
                    Title = "Rapid Prototyping Platform",
                    Description = "Affordable AI-based prototyping solutions to speed up design, engineering, and innovation.",
                    Industry = "Manufacturing / Product Design",
                    Technologies = "Python, TensorFlow, 3D Visualization",
                    IsFeatured = true
                },
                new Solution
                {
                    Title = "Digital Employee Experience Monitor",
                    Description = "Proactively identifies issues impacting digital employee experience and recommends fixes.",
                    Industry = "Human Resources",
                    Technologies = "React, .NET Core, SQL Server",
                    IsFeatured = false
                }
            );
            await context.SaveChangesAsync();
        }

        // Seed Case Studies
        if (!context.CaseStudies.Any())
        {
            context.CaseStudies.AddRange(
                new CaseStudy
                {
                    SolutionId = 1,
                    Title = "Virtual Assistant for NHS Trust",
                    Description = "Deployed an AI assistant to handle internal HR and IT queries, reducing support ticket volume.",
                    ClientName = "Sunderland Health Alliance",
                    Industry = "Healthcare",
                    Outcome = "40% reduction in support tickets; 24/7 employee assistance available."
                },
                new CaseStudy
                {
                    SolutionId = 2,
                    Title = "Prototyping Tool for Auto Manufacturer",
                    Description = "Built a generative design tool to rapidly prototype vehicle interior components.",
                    ClientName = "North East Motors",
                    Industry = "Automotive",
                    Outcome = "Reduced concept-to-prototype time from 6 weeks to 5 days."
                }
            );
            await context.SaveChangesAsync();
        }

        // Seed Feedback
        if (!context.Feedbacks.Any())
        {
            context.Feedbacks.AddRange(
                new Feedback
                {
                    CustomerName = "Sarah Johnson",
                    CompanyName = "Sunderland Health Alliance",
                    Rating = 5,
                    Comment = "The virtual assistant transformed our internal support. Highly recommended!",
                    IsApproved = true
                },
                new Feedback
                {
                    CustomerName = "Michael Brown",
                    CompanyName = "North East Motors",
                    Rating = 4,
                    Comment = "Great prototyping platform with fast turnaround times.",
                    IsApproved = true
                }
            );
            await context.SaveChangesAsync();
        }

        // Seed Articles
        if (!context.Articles.Any())
        {
            context.Articles.AddRange(
                new Article
                {
                    Title = "The Future of Digital Employee Experience",
                    Author = "AI-Solutions Team",
                    Content = "Artificial Intelligence is reshaping how employees interact with workplace technology...",
                    IsPublished = true
                },
                new Article
                {
                    Title = "How AI Speeds Up Product Prototyping",
                    Author = "Engineering Lead",
                    Content = "Traditional prototyping is costly and slow. AI-driven generative design is changing the game...",
                    IsPublished = true
                }
            );
            await context.SaveChangesAsync();
        }

        // Seed Events
        if (!context.Events.Any())
        {
            context.Events.AddRange(
                new Event
                {
                    Title = "AI in the Workplace Expo 2026",
                    Description = "A promotional event showcasing our virtual assistant and prototyping solutions.",
                    EventDate = new DateTime(2026, 9, 15),
                    Location = "Sunderland Software Centre",
                    EventType = "Upcoming"
                },
                new Event
                {
                    Title = "Product Launch 2025",
                    Description = "Launch event for our new digital employee experience monitor.",
                    EventDate = new DateTime(2025, 11, 20),
                    Location = "University of Sunderland",
                    EventType = "Past"
                }
            );
            await context.SaveChangesAsync();

            // Seed Event Gallery Images for the first past event
            if (!context.EventImages.Any())
            {
                var pastEvent = context.Events.FirstOrDefault(e => e.EventType == "Past");
                if (pastEvent != null)
                {
                    context.EventImages.AddRange(
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event1.jpg", Caption = "Product launch keynote" },
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event2.jpg", Caption = "Live demo session" },
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event3.jpg", Caption = "Networking with attendees" },
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event4.jpg", Caption = "Exhibition booth" },
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event5.jpg", Caption = "Team photo" },
                        new EventImage { EventId = pastEvent.EventId, ImagePath = "/images/events/event6.jpg", Caption = "Award presentation" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }

        // Seed Inquiries
        if (!context.Inquiries.Any())
        {
            var customer = new Customer
            {
                FullName = "Demo Customer",
                Email = "demo@example.com",
                Phone = "+441234567890",
                CompanyName = "Demo Ltd",
                Country = "United Kingdom"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            context.Inquiries.Add(new Inquiry
            {
                CustomerId = customer.CustomerId,
                JobTitle = "AI Workflow Automation",
                JobCategory = "AI & Machine Learning",
                JobDetails = "Looking for an AI assistant to handle HR and IT employee queries.",
                Status = "New"
            });
            await context.SaveChangesAsync();
        }
    }
}
