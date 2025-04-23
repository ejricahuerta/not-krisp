using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotKrisp.API.Services;
using NotKrisp.API.Services.Interfaces;
using System.IO;

namespace NotKrisp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AudioController : ControllerBase
    {
        private readonly SpeechToTextService _speechToTextService;
        private readonly ILogger<AudioController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AudioController(
            SpeechToTextService speechToTextService,
            ILogger<AudioController> logger,
            IWebHostEnvironment environment)
        {
            _speechToTextService = speechToTextService;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudio(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file was uploaded.");
                }

                // TODO: Change this to use Azure Blob Storage or a dedicated file server
                // Currently saving to local wwwroot/uploads which is not suitable for production
                // Consider implementing:
                // 1. Azure Blob Storage for cloud storage
                // 2. A dedicated file server with proper backup
                // 3. Temporary storage with cleanup job
                // 4. Proper file access controls and security

                // Use ContentRootPath instead of WebRootPath and create a dedicated uploads directory
                var uploadsDir = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate a unique filename
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL for the saved file
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                _logger.LogInformation("Audio file uploaded successfully: {FileName}", fileName);

                return Ok(new { audioUrl = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading audio file");
                return StatusCode(500, "An error occurred while uploading the file.");
            }
        }

        [HttpPost("transcribe")]
        public async Task<IActionResult> TranscribeAudio(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file was uploaded.");
                }

                // Convert the uploaded file to a byte array
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position to start of stream

                // Transcribe the audio
                var transcription = await _speechToTextService.TranscribeAudioAsync(
                    memoryStream,
                    file.FileName,
                    CancellationToken.None
                );

                _logger.LogInformation("Audio transcription completed successfully");

                return Ok(new { transcription });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transcribing audio");
                return StatusCode(500, "An error occurred while transcribing the audio.");
            }
        }
    }
}