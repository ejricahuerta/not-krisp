namespace NotKrisp.API.Models;

public class AppSettings
{
    public AssemblyAISettings AssemblyAI { get; set; } = new();
}

public class AssemblyAISettings
{
    public string ApiKey { get; set; } = string.Empty;
} 