using Microsoft.Playwright;

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

    public Task<int> GetCartItemCountAsync() => _cartItems.CountAsync();

    public async Task<bool> IsProductInCartAsync(string productName) =>
        await _productNames.GetByText(productName, new() { Exact = true }).CountAsync() > 0;

    public async Task RemoveProductAsync(string productName)
    {
        var cartItem = _cartItems.Filter(new()
        {
            HasTextString = productName
        });

        await cartItem.GetByRole(AriaRole.Button, new() { NameString = "Remove" }).ClickAsync();
    }

    public Task ContinueShoppingAsync() => _continueShoppingButton.ClickAsync();

    public Task CheckoutAsync() => _checkoutButton.ClickAsync();
}
