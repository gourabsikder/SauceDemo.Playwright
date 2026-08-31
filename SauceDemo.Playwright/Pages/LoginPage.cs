using Microsoft.Playwright;
using SauceDemo.Playwright.Logging;

namespace SauceDemo.Playwright.Pages;

public class LoginPage
{
    private readonly IPage _page;
    private readonly ILocator _usernameInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;
    private readonly ILocator _errorMessage;

    public LoginPage(IPage page)
    {
        _page = page;
        _usernameInput = _page.Locator("#user-name");
        _passwordInput = _page.Locator("#password");
        _loginButton = _page.Locator("#login-button");
        _errorMessage = _page.Locator("[data-test='error']");
    }

    public async Task NavigateToLoginPageAsync(Uri loginUrl)
    {
        TestLogger.LogInformation($"Navigating to {loginUrl.AbsoluteUri}.");
        await _page.GotoAsync(loginUrl.AbsoluteUri, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    public async Task LoginAsync(string username, string password)
    {
        TestLogger.LogInformation("Submitting login credentials.");
        await _usernameInput.FillAsync(username);
        await _passwordInput.FillAsync(password);
        await _loginButton.ClickAsync();
    }

    public Task<string> GetErrorMessageAsync()
    {
        TestLogger.LogInformation("Reading login error message.");
        return _errorMessage.InnerTextAsync();
    }
}
