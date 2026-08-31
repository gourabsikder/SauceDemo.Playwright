using Microsoft.Playwright;
using SauceDemo.Playwright.Logging;

namespace SauceDemo.Playwright.Pages;

public class CheckoutPage
{
    private readonly IPage _page;
    private readonly ILocator _firstNameInput;
    private readonly ILocator _lastNameInput;
    private readonly ILocator _postalCodeInput;
    private readonly ILocator _continueButton;
    private readonly ILocator _checkoutOverview;
    private readonly ILocator _finishButton;
    private readonly ILocator _orderConfirmationMessage;
    private readonly ILocator _backHomeButton;

    public CheckoutPage(IPage page)
    {
        _page = page;
        _firstNameInput = _page.Locator("[data-test='firstName']");
        _lastNameInput = _page.Locator("[data-test='lastName']");
        _postalCodeInput = _page.Locator("[data-test='postalCode']");
        _continueButton = _page.Locator("[data-test='continue']");
        _checkoutOverview = _page.Locator(".summary_info");
        _finishButton = _page.Locator("[data-test='finish']");
        _orderConfirmationMessage = _page.Locator("[data-test='complete-header']");
        _backHomeButton = _page.Locator("[data-test='back-to-products']");
    }

    public async Task EnterCustomerInformationAsync(string firstName, string lastName, string postalCode)
    {
        TestLogger.LogInformation("Entering checkout customer information.");
        await _firstNameInput.FillAsync(firstName);
        await _lastNameInput.FillAsync(lastName);
        await _postalCodeInput.FillAsync(postalCode);
    }

    public Task ContinueToOverviewAsync()
    {
        TestLogger.LogInformation("Continuing to checkout overview.");
        return _continueButton.ClickAsync();
    }

    public async Task<bool> IsCheckoutOverviewDisplayedAsync()
    {
        TestLogger.LogInformation("Checking checkout overview visibility.");
        await _checkoutOverview.WaitForAsync();
        return await _checkoutOverview.IsVisibleAsync();
    }

    public async Task FinishOrderAsync()
    {
        TestLogger.LogInformation("Finishing the order.");
        await _checkoutOverview.WaitForAsync();
        await _finishButton.ClickAsync();
    }

    public Task<string> GetOrderConfirmationMessageAsync()
    {
        TestLogger.LogInformation("Reading order confirmation message.");
        return _orderConfirmationMessage.InnerTextAsync();
    }

    public Task BackToHomeAsync()
    {
        TestLogger.LogInformation("Returning to products from order confirmation.");
        return _backHomeButton.ClickAsync();
    }
}
