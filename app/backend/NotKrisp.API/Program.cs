using Microsoft.EntityFrameworkCore;
using NotKrisp.API.Data;
using NotKrisp.API.Models;
using NotKrisp.API.Services;
using NotKrisp.API.Services.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure PostgreSQL connection
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var connectionString = $"Host={uri.Host};Port={5432};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
}
else
{
    var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Connection string not found");
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<SpeechToTextService>();

// Add HttpClient
builder.Services.AddHttpClient();

// Configure AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration);

// Validate AssemblyAI configuration
var assemblyAIConfig = builder.Configuration.GetSection("AssemblyAI").Get<AssemblyAISettings>();
if (string.IsNullOrWhiteSpace(assemblyAIConfig?.ApiKey))
{
    throw new InvalidOperationException("AssemblyAI API key is not configured. Please check your appsettings.json file.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();

// Configure file upload limits
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Add global error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

// Create uploads directory if it doesn't exist
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Add static file serving
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.MapControllers();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();


