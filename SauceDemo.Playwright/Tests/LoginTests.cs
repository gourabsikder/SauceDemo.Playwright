using NUnit.Framework;
using SauceDemo.Playwright.Base;
using SauceDemo.Playwright.Pages;

namespace SauceDemo.Playwright.Tests;

[TestFixture]
public class LoginTests : BaseTest
{
    [Test]
    public async Task ValidLogin_ShouldNavigateToProductsPage()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        await Expect(Page).ToHaveURLAsync(Configuration.GetUrl(Configuration.InventoryPath).AbsoluteUri);
    }

    [Test]
    public async Task InvalidLogin_ShouldDisplayErrorMessage()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.InvalidUser.Username, Configuration.InvalidUser.Password);

        var errorMessage = await loginPage.GetErrorMessageAsync();

        Assert.That(errorMessage, Does.Contain(Configuration.InvalidCredentialsMessage));
    }

    [Test]
    public async Task LockedOutUser_ShouldDisplayErrorMessage()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.LockedOutUser.Username, Configuration.LockedOutUser.Password);

        var errorMessage = await loginPage.GetErrorMessageAsync();

        Assert.That(errorMessage, Does.Contain(Configuration.LockedOutMessage));
    }
}
