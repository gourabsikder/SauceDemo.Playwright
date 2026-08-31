using NUnit.Framework;
using SauceDemo.Playwright.Base;
using SauceDemo.Playwright.Pages;

namespace SauceDemo.Playwright.Tests;

[TestFixture]
public class InventoryTests : BaseTest
{
    [Test]
    public async Task InventoryPage_ShouldDisplayProducts()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        var inventoryPage = new InventoryPage(Page);

        Logger.Information("Verifying navigation to the inventory page.");
        await Expect(Page).ToHaveURLAsync(Configuration.GetUrl(Configuration.InventoryPath).AbsoluteUri);
        Logger.Information("Verifying inventory page title.");
        Assert.That(await inventoryPage.GetPageTitleAsync(), Is.EqualTo("Products"));
        Logger.Information("Verifying at least one inventory product is displayed.");
        Assert.That(await inventoryPage.GetProductCountAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task AddProductToCart_ShouldUpdateCart()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        var inventoryPage = new InventoryPage(Page);

        await inventoryPage.AddProductToCartAsync("Sauce Labs Backpack");
        await inventoryPage.GoToCartAsync();

        Logger.Information("Verifying navigation to the cart page.");
        await Expect(Page).ToHaveURLAsync(Configuration.GetUrl("cart.html").AbsoluteUri);
    }
}
