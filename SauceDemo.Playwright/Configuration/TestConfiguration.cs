using Microsoft.Extensions.Configuration;

namespace SauceDemo.Playwright.Configuration;

public sealed class TestConfiguration
{
    private readonly IConfiguration _configuration;

    public TestConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(Path.Combine("Configuration", "appsettings.json"), optional: false)
            .AddJsonFile(Path.Combine("Configuration", $"appsettings.{GetEnvironmentName()}.json"), optional: true)
            .AddEnvironmentVariables(prefix: "SAUCEDEMO_")
            .Build();

        BaseUrl = GetRequiredUri("BaseUrl");
        InventoryPath = GetRequiredValue("InventoryPath");
        StandardUser = GetUser("Standard");
        InvalidUser = GetUser("Invalid");
        LockedOutUser = GetUser("LockedOut");
        InvalidCredentialsMessage = GetRequiredValue("ExpectedMessages:InvalidCredentials");
        LockedOutMessage = GetRequiredValue("ExpectedMessages:LockedOut");
        BrowserSettings = new BrowserSettings(
            GetRequiredBrowserSetting("Browser"),
            GetRequiredBrowserSettingBoolean("Headless"));
    }

    public Uri BaseUrl { get; }
    public string InventoryPath { get; }
    public TestUser StandardUser { get; }
    public TestUser InvalidUser { get; }
    public TestUser LockedOutUser { get; }
    public string InvalidCredentialsMessage { get; }
    public string LockedOutMessage { get; }
    public BrowserSettings BrowserSettings { get; }

    public Uri GetUrl(string relativePath) => new(BaseUrl, relativePath);

    private static string GetEnvironmentName() =>
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

    private string GetRequiredValue(string key) =>
        _configuration[$"TestSettings:{key}"]
        ?? throw new InvalidOperationException($"TestSettings:{key} is not configured.");

    private Uri GetRequiredUri(string key)
    {
        var value = GetRequiredValue(key);

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"TestSettings:{key} must be an absolute URL.");
        }

        return uri.AbsoluteUri.EndsWith('/') ? uri : new Uri($"{uri.AbsoluteUri}/");
    }

    private TestUser GetUser(string userType) => new(
        GetRequiredValue($"Users:{userType}:Username"),
        GetRequiredValue($"Users:{userType}:Password"));

    private string GetRequiredBrowserSetting(string key) =>
        _configuration[$"BrowserSettings:{key}"]
        ?? throw new InvalidOperationException($"BrowserSettings:{key} is not configured.");

    private bool GetRequiredBrowserSettingBoolean(string key)
    {
        var value = GetRequiredBrowserSetting(key);

        if (!bool.TryParse(value, out var result))
        {
            throw new InvalidOperationException($"BrowserSettings:{key} must be true or false.");
        }

        return result;
    }
}

public sealed record TestUser(string Username, string Password);
public sealed record BrowserSettings(string Browser, bool Headless);
