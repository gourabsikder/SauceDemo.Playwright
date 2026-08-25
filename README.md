# SauceDemo Playwright Tests

## Configuration

Default test settings are in `SauceDemo.Playwright/Configuration/appsettings.json`. For local or environment-specific settings, create `appsettings.{DOTNET_ENVIRONMENT}.json` in that directory. The file is copied to the test output automatically.

Environment variables override JSON settings. Use the `SAUCEDEMO_` prefix and double underscores for nested settings. For example:

```powershell
$env:SAUCEDEMO_TestSettings__BaseUrl = "https://www.saucedemo.com/"
$env:SAUCEDEMO_TestSettings__Users__Standard__Username = "standard_user"
$env:SAUCEDEMO_TestSettings__Users__Standard__Password = "secret_sauce"
dotnet test
```

Run the suite from the repository root:

```powershell
dotnet test SauceDemo.Playwright.slnx
```
