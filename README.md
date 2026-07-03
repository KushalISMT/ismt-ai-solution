# AI-Solutions Web Portal

AI-Solutions Web Portal is an ASP.NET Core MVC application that showcases AI software solutions, case studies, articles, and events for a fictitious client. Visitors can submit project inquiries through a validated Contact Us form, while administrators can view, analyse, and export inquiry data from a secure dashboard.

A prototype built for the **CET333 Product Development** assignment (Computer Systems Engineering track).

## Technology Stack

- .NET 9 (ASP.NET Core MVC)
- Entity Framework Core 9
- Microsoft SQL Server
- ASP.NET Core Identity (admin authentication)
- Bootstrap 5

## Project Structure

```text
AI-Solutions.Portal/
+-- AI-Solutions.Portal.Web/     ASP.NET Core MVC project
|   +-- Controllers/              Public + Admin controllers
|   +-- Data/                     DbContext, migrations, seed data
|   +-- Models/Entities/          Domain models
|   +-- Models/ViewModels/        Form view models
|   +-- Views/                    Razor views
|   +-- appsettings.json          Connection string
|   +-- Program.cs
+-- AI-Solutions.Portal.Tests/   xUnit integration tests
+-- nuget.config                  Restricts NuGet to nuget.org only
```

## Database Connection

The application uses the following connection string (stored in `appsettings.json`):

```json
"Server=.;User Id=dbadmin;Password=Irush@12345;Initial Catalog=ai-solutions-db;TrustServerCertificate=True;"
```

## Running Tests

An xUnit integration test project is included. It uses a separate test database (`ai-solutions-db-test`) so it does not affect your production data.

```bash
cd AI-Solutions.Portal
dotnet test AI-Solutions.Portal.Tests/AI-Solutions.Portal.Tests.csproj
```

Latest result: **11 tests passed** on .NET 9 (8 public page URL checks, 1 admin auth check, 2 Contact Us form checks).

## How to Run

1. Open the solution in **Visual Studio 2022** or run from the command line.
2. Ensure SQL Server is running and the `dbadmin` login exists.
3. From the project root folder, run:

```bash
cd AI-Solutions.Portal.Web
dotnet run
```

4. The application will start on:
   - HTTP: `http://localhost:7000`
   - HTTPS: `https://localhost:7001`

5. On first run, EF Core migrations are applied automatically and sample data is seeded.

## Default Admin Credentials

- **Email:** `admin@ai-solutions.com`
- **Password:** `Admin@123`

## Public Pages

| Page | URL |
|---|---|
| Home | `/` |
| Software Solutions | `/Solutions` |
| Case Studies | `/CaseStudies` |
| Customer Feedback | `/Feedback` |
| Articles | `/Articles` |
| Upcoming Events | `/Events/Upcoming` |
| Event Gallery | `/Events/Gallery` |
| Contact Us | `/Contact` |

## Admin Area

| Page | URL |
|---|---|
| Dashboard | `/Admin` |
| Customer Inquiries | `/Admin/Inquiries` |
| Export CSV | `/Admin/ExportCsv` |

## Features Implemented

- Software solutions showcase
- Past solutions / case studies
- Customer feedback with 1-5 star ratings
- Promotional articles / blog
- Upcoming events and event photo gallery
- Contact Us form with job title, job details, searchable country list, and phone validation
- Password-protected admin dashboard
- Inquiry analytics (by country, by job title)
- CSV export of inquiries
- Responsive Bootstrap layout
- SQL Server database with Entity Framework Core

## Tested Endpoints

All public pages and the admin area were tested and return HTTP 200.
The Contact Us form successfully saves inquiries to the database.
The admin login restricts access to authenticated users in the `Admin` role.
