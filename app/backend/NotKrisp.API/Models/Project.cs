namespace NotKrisp.API.Models;

public class Project
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
    public int Number { get; set; }
    public string State { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
    public List<Label> Labels { get; set; } = new();
    public List<Assignee> Assignees { get; set; } = new();
} 