using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Configuration;

namespace SauceDemo.Playwright.Base;

public class BaseTest
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    protected TestConfiguration Configuration { get; } = new();

    protected IPage Page { get; private set; } = null!;

    protected virtual BrowserTypeLaunchOptions BrowserLaunchOptions => new()
    {
        Headless = false
    };

    [SetUp]
    public async Task SetUpAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(BrowserLaunchOptions);
        _context = await _browser.NewContextAsync();
        Page = await _context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
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
    }

    protected static IPageAssertions Expect(IPage page) => Assertions.Expect(page);
}
