using NUnit.Framework;
using SauceDemo.Playwright.Base;
using SauceDemo.Playwright.Pages;

namespace SauceDemo.Playwright.Tests;

[TestFixture]
public class CartTests : BaseTest
{
    [Test]
    public async Task AddProductToCart_ShouldDisplayProductInCart()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        var inventoryPage = new InventoryPage(Page);

        await inventoryPage.AddProductToCartAsync("Sauce Labs Backpack");
        await inventoryPage.GoToCartAsync();

        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.IsProductInCartAsync("Sauce Labs Backpack"), Is.True);
        Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task RemoveProductFromCart_ShouldRemoveProduct()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        var inventoryPage = new InventoryPage(Page);

        await inventoryPage.AddProductToCartAsync("Sauce Labs Backpack");
        await inventoryPage.GoToCartAsync();

        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.IsProductInCartAsync("Sauce Labs Backpack"), Is.True);

        await cartPage.RemoveProductAsync("Sauce Labs Backpack");

        Assert.That(await cartPage.IsProductInCartAsync("Sauce Labs Backpack"), Is.False);
        Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(0));
    }
}
