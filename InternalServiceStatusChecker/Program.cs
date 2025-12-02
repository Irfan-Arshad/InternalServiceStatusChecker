using System.Net.Http;

// Build the app
var builder = WebApplication.CreateBuilder(args);

// Register HttpClient for status checks
builder.Services.AddHttpClient("StatusChecker", client =>
{
    client.Timeout = TimeSpan.FromSeconds(3);
});

var app = builder.Build();

// List of services to check
// TODO: Replace URLs with real internal endpoints later if needed.
var servicesToCheck = new List<(string Name, string Url)>
{
    ("Room Booking System",  "https://rooms.example.local/health"),
    ("Patient Listing Form", "https://patlist.example.local/health"),
    ("Intranet Home",        "https://intranet.example.local/")
};

// Health endpoint - checks the API itself
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow
    });
});

// Check endpoint - pings each configured service
app.MapGet("/check", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("StatusChecker");
    var tasks = servicesToCheck.Select(async svc =>
    {
        try
        {
            using var response = await client.GetAsync(svc.Url);
            var isUp = response.IsSuccessStatusCode;

            return new
            {
                svc.Name,
                svc.Url,
                IsUp = isUp,
                StatusCode = (int?)response.StatusCode,
                Error = isUp ? null : $"Non-success status code: {(int)response.StatusCode}",
                CheckedAtUtc = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new
            {
                svc.Name,
                svc.Url,
                IsUp = false,
                StatusCode = (int?)null,
                Error = ex.Message,
                CheckedAtUtc = DateTime.UtcNow
            };
        }
    });
    var results = await Task.WhenAll(tasks);
    return Results.Ok(results);
});

// Root endpoint - simple info
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        application = "Internal Service Status Checker",
        description = "Checks internal systems and reports whether they are up or down.",
        healthEndpoint = "/health",
        checkEndpoint = "/check"
    });
});

// Run the app
app.Run();
