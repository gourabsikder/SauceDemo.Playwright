using NUnit.Framework;
using SauceDemo.Playwright.Base;
using SauceDemo.Playwright.Pages;

namespace SauceDemo.Playwright.Tests;

[TestFixture]
public class CheckoutTests : BaseTest
{
    [Test]
    public async Task CompletePurchase_ShouldDisplayOrderConfirmation()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateToLoginPageAsync(Configuration.BaseUrl);
        await loginPage.LoginAsync(Configuration.StandardUser.Username, Configuration.StandardUser.Password);

        var inventoryPage = new InventoryPage(Page);

        await inventoryPage.AddProductToCartAsync("Sauce Labs Backpack");
        await inventoryPage.GoToCartAsync();

        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.IsProductInCartAsync("Sauce Labs Backpack"), Is.True);

        await cartPage.CheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);

        await checkoutPage.EnterCustomerInformationAsync("Gourab", "Tester", "560001");
        await checkoutPage.ContinueToOverviewAsync();

        Assert.That(await checkoutPage.IsCheckoutOverviewDisplayedAsync(), Is.True);

        await checkoutPage.FinishOrderAsync();

        var orderConfirmationMessage = await checkoutPage.GetOrderConfirmationMessageAsync();

        Assert.That(orderConfirmationMessage, Does.Contain("Thank you for your order"));
    }
}
