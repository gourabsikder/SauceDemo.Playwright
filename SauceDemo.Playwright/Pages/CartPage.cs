using Microsoft.Playwright;
using SauceDemo.Playwright.Logging;

namespace SauceDemo.Playwright.Pages;

public class CartPage
{
    private readonly IPage _page;
    private readonly ILocator _cartContainer;
    private readonly ILocator _cartItems;
    private readonly ILocator _productNames;
    private readonly ILocator _productQuantities;
    private readonly ILocator _continueShoppingButton;
    private readonly ILocator _checkoutButton;

    public CartPage(IPage page)
    {
        _page = page;
        _cartContainer = _page.Locator("[data-test='cart-list']");
        _cartItems = _cartContainer.Locator("[data-test='inventory-item']");
        _productNames = _cartItems.Locator("[data-test='inventory-item-name']");
        _productQuantities = _cartItems.Locator("[data-test='item-quantity']");
        _continueShoppingButton = _page.Locator("[data-test='continue-shopping']");
        _checkoutButton = _page.Locator("[data-test='checkout']");
    }

    public Task<int> GetCartItemCountAsync()
    {
        TestLogger.LogInformation("Counting cart items.");
        return _cartItems.CountAsync();
    }

    public async Task<bool> IsProductInCartAsync(string productName)
    {
        TestLogger.LogInformation($"Checking whether product is in the cart: {productName}.");
        return await _productNames.GetByText(productName, new() { Exact = true }).CountAsync() > 0;
    }

    public async Task RemoveProductAsync(string productName)
    {
        TestLogger.LogInformation($"Removing product from cart: {productName}.");
        var cartItem = _cartItems.Filter(new()
        {
            HasTextString = productName
        });

        await cartItem.GetByRole(AriaRole.Button, new() { NameString = "Remove" }).ClickAsync();
    }

    public Task ContinueShoppingAsync()
    {
        TestLogger.LogInformation("Continuing shopping from the cart.");
        return _continueShoppingButton.ClickAsync();
    }

    public Task CheckoutAsync()
    {
        TestLogger.LogInformation("Proceeding to checkout.");
        return _checkoutButton.ClickAsync();
    }
}
