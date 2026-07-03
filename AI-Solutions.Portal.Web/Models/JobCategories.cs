namespace AI_Solutions.Portal.Web.Models;

public static class JobCategories
{
    public static IReadOnlyList<string> All { get; } = new List<string>
    {
        "AI & Machine Learning",
        "Web Development",
        "Mobile Development",
        "Cloud & DevOps",
        "Data Engineering",
        "Cybersecurity",
        "IT Support & Helpdesk",
        "Project Management",
        "UI/UX Design",
        "Quality Assurance"
    }.AsReadOnly();
}
