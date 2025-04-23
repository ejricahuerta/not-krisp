using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotKrisp.API.Models;

public class Issue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
    public List<Label> Labels { get; set; } = new();
    public List<Assignee> Assignees { get; set; } = new();
}

public class Label
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class Assignee
{
    public string Login { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}