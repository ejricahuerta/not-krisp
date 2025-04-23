using AssemblyAI;
using AssemblyAI.Transcripts;
using Microsoft.Extensions.Options;
using NotKrisp.API.Models;

namespace NotKrisp.API.Services;

public class SpeechToTextService
{
    private readonly ILogger<SpeechToTextService> _logger;
    private readonly AssemblyAIClient _client;

    public SpeechToTextService(
        ILogger<SpeechToTextService> logger,
        IOptions<AppSettings> settings)
    {
        _logger = logger;
        
        // Validate API key
        var apiKey = settings.Value.AssemblyAI.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("AssemblyAI API key is not configured. Please check your appsettings.json file.");
        }
        
        _logger.LogInformation("Initializing SpeechToTextService with API key: {ApiKey}", 
            apiKey.Substring(0, 4) + "...");
        
        // Initialize AssemblyAI client
        _client = new AssemblyAIClient(apiKey);
    }

    public async Task<string> TranscribeAudioAsync(Stream audioStream, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting audio transcription for file: {FileName}", fileName);
            
            // Save the stream to a temporary file
            var tempFilePath = Path.GetTempFileName();
            try
            {
                using (var fileStream = File.Create(tempFilePath))
                {
                    await audioStream.CopyToAsync(fileStream, cancellationToken);
                }

                // Transcribe the audio file
                var transcript = await _client.Transcripts.TranscribeAsync(new FileInfo(tempFilePath));
                
                // Wait for completion
                transcript.EnsureStatusCompleted();
                
                _logger.LogInformation("Transcription completed successfully");
                return transcript.Text;
            }
            finally
            {
                // Clean up the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transcribing audio file: {FileName}. Error details: {Error}", 
                fileName, ex.Message);
            throw;
        }
    }
} 