using Microsoft.Playwright.NUnit;
using SauceDemo.Playwright.Configuration;

namespace SauceDemo.Playwright.Base;

public class BaseTest : PageTest
{
    protected TestConfiguration Configuration { get; } = new();
}
