using Microsoft.Playwright;
using SauceDemo.Playwright.Logging;

namespace SauceDemo.Playwright.Pages;

public class InventoryPage
{
    private readonly IPage _page;
    private readonly ILocator _pageTitle;
    private readonly ILocator _inventoryContainer;
    private readonly ILocator _productItems;
    private readonly ILocator _shoppingCartLink;

    public InventoryPage(IPage page)
    {
        _page = page;
        _pageTitle = _page.Locator("[data-test='title']");
        _inventoryContainer = _page.Locator("[data-test='inventory-container']");
        _productItems = _inventoryContainer.Locator("[data-test='inventory-item']");
        _shoppingCartLink = _page.Locator("[data-test='shopping-cart-link']");
    }

    public Task<string> GetPageTitleAsync()
    {
        TestLogger.LogInformation("Reading inventory page title.");
        return _pageTitle.InnerTextAsync();
    }

    public Task<int> GetProductCountAsync()
    {
        TestLogger.LogInformation("Counting inventory products.");
        return _productItems.CountAsync();
    }

    public async Task AddProductToCartAsync(string productName)
    {
        TestLogger.LogInformation($"Adding product to cart: {productName}.");
        var productItem = _productItems.Filter(new()
        {
            HasTextString = productName
        });

        await productItem.GetByRole(AriaRole.Button, new() { NameString = "Add to cart" }).ClickAsync();
    }

    public Task GoToCartAsync()
    {
        TestLogger.LogInformation("Navigating to the cart.");
        return _shoppingCartLink.ClickAsync();
    }
}
