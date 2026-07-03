using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Tests;

public class PublicPagesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PublicPagesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Ensure test database is created and migrated
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Solutions")]
    [InlineData("/CaseStudies")]
    [InlineData("/Feedback")]
    [InlineData("/Articles")]
    [InlineData("/Events/Upcoming")]
    [InlineData("/Events/Gallery")]
    [InlineData("/Contact")]
    public async Task Public_Pages_Return_Success(string url)
    {
        var response = await _client.GetAsync(url, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Page_Requires_Authentication()
    {
        var response = await _client.GetAsync("/Admin", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Contact_Form_Submits_Successfully()
    {
        // Arrange
        var getResponse = await _client.GetAsync("/Contact", TestContext.Current.CancellationToken);
        getResponse.EnsureSuccessStatusCode();
        var content = await getResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var token = ExtractAntiForgeryToken(content);

        var formData = new Dictionary<string, string>
        {
            { "FullName", "Test User" },
            { "Email", "testuser@example.com" },
            { "Phone", "+441234567890" },
            { "CompanyName", "Test Ltd" },
            { "Country", "United Kingdom" },
            { "JobCategory", "Cloud & DevOps" },
            { "JobTitle", "DevOps Engineer" },
            { "JobDetails", "Need AI automation help" },
            { "__RequestVerificationToken", token }
        };

        // Act
        var postResponse = await _client.PostAsync("/Contact/Submit", new FormUrlEncodedContent(formData), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
        Assert.Equal("/Contact/ThankYou", postResponse.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Contact_Form_Rejects_Invalid_Email()
    {
        var getResponse = await _client.GetAsync("/Contact", TestContext.Current.CancellationToken);
        var content = await getResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var token = ExtractAntiForgeryToken(content);

        var formData = new Dictionary<string, string>
        {
            { "FullName", "Test User" },
            { "Email", "invalid-email" },
            { "Phone", "+441234567890" },
            { "CompanyName", "Test Ltd" },
            { "Country", "United Kingdom" },
            { "JobCategory", "Cloud & DevOps" },
            { "JobTitle", "DevOps Engineer" },
            { "JobDetails", "Need AI automation help" },
            { "__RequestVerificationToken", token }
        };

        var postResponse = await _client.PostAsync("/Contact/Submit", new FormUrlEncodedContent(formData), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        var responseContent = await postResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("The Email field is not a valid e-mail address.", responseContent);
    }

    private static string ExtractAntiForgeryToken(string html)
    {
        var match = Regex.Match(html, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]*)""");
        return match.Groups[1].Value;
    }
}
