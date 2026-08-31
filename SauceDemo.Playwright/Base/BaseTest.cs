using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Configuration;
using SauceDemo.Playwright.Logging;

namespace SauceDemo.Playwright.Base;

public class BaseTest
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private bool _tracingStarted;

    protected TestConfiguration Configuration { get; } = new();

    protected TestLogger Logger { get; private set; } = null!;

    protected IPage Page { get; private set; } = null!;

    protected virtual BrowserTypeLaunchOptions BrowserLaunchOptions => new()
    {
        Headless = Configuration.BrowserSettings.Headless
    };

    [SetUp]
    public async Task SetUpAsync()
    {
        Logger = TestLogger.CreateForCurrentTest();
        TestLogger.SetCurrent(Logger);
        Logger.Information($"Test started: {TestContext.CurrentContext.Test.Name}");

        try
        {
            Logger.Information("Initializing Playwright.");
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            Logger.Information($"Launching {Configuration.BrowserSettings.Browser} browser. Headless: {Configuration.BrowserSettings.Headless}.");
            _browser = await GetBrowserType().LaunchAsync(BrowserLaunchOptions);

            Logger.Information("Creating browser context.");
            _context = await _browser.NewContextAsync();
            await _context.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            _tracingStarted = true;

            Logger.Information("Creating page.");
            Page = await _context.NewPageAsync();
        }
        catch (Exception exception)
        {
            Logger.Error("Browser initialization failed.", exception);
            throw;
        }
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                Logger.Information("Test passed. Stopping trace without saving it.");
                await StopTracingAsync();
            }
            else
            {
                Logger.Error($"Test failed: {TestContext.CurrentContext.Result.Message}");
                await SaveFailureArtifactsAsync();
            }
        }
        finally
        {
            // Closing the context also closes its pages.
            if (_context is not null)
            {
                await _context.CloseAsync();
            }

            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();
            Logger.Information("Test completed. Browser resources disposed.");
            TestLogger.SetCurrent(null);
        }
    }

    protected static IPageAssertions Expect(IPage page) => Assertions.Expect(page);

    private IBrowserType GetBrowserType() =>
        Configuration.BrowserSettings.Browser.Trim().ToUpperInvariant() switch
        {
            "CHROMIUM" => _playwright!.Chromium,
            "FIREFOX" => _playwright!.Firefox,
            "WEBKIT" => _playwright!.Webkit,
            _ => throw new InvalidOperationException(
                $"BrowserSettings:Browser '{Configuration.BrowserSettings.Browser}' is not supported. " +
                "Supported values are Chromium, Firefox, and WebKit.")
        };

    private async Task SaveFailureArtifactsAsync()
    {
        try
        {
            Logger.Warning("Saving failure diagnostics.");
            var artifactDirectory = GetArtifactDirectory();
            Directory.CreateDirectory(artifactDirectory);

            var artifactName = GetArtifactName();
            var screenshotPath = Path.Combine(artifactDirectory, $"{artifactName}.png");
            var tracePath = Path.Combine(artifactDirectory, $"{artifactName}.zip");

            if (Page is not null)
            {
                try
                {
                    await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
                }
                catch
                {
                    // Diagnostics must not hide the original test failure.
                    Logger.Warning("Unable to save failure screenshot.");
                }
            }

            await StopTracingAsync(tracePath);
        }
        catch
        {
            Logger.Warning("Unable to save failure artifacts.");
            await StopTracingAsync();
        }
    }

    private async Task StopTracingAsync(string? tracePath = null)
    {
        if (!_tracingStarted || _context is null)
        {
            return;
        }

        try
        {
            if (tracePath is null)
            {
                await _context.Tracing.StopAsync();
            }
            else
            {
                await _context.Tracing.StopAsync(new() { Path = tracePath });
            }
        }
        catch
        {
            // Diagnostics must not hide the original test failure.
            Logger.Warning("Unable to stop Playwright tracing.");
        }
        finally
        {
            _tracingStarted = false;
        }
    }

    private static string GetArtifactName()
    {
        var test = TestContext.CurrentContext.Test;
        var className = test.ClassName?.Split('.').LastOrDefault() ?? "UnknownTestClass";
        var methodName = test.MethodName ?? test.Name;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

        return $"{SanitizeFileName(className)}_{SanitizeFileName(methodName)}_{timestamp}";
    }

    private static string GetArtifactDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (directory.EnumerateFiles("*.csproj").Any())
            {
                return Path.Combine(directory.FullName, "TestResults", "Artifacts");
            }

            directory = directory.Parent;
        }

        return Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "Artifacts");
    }

    private static string SanitizeFileName(string value)
    {
        foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidCharacter, '_');
        }

        return value;
    }
}
