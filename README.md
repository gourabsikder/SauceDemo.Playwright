# SauceDemo Playwright Automation Framework

[![Playwright Tests](https://github.com/gourabsikder/SauceDemo.Playwright/actions/workflows/main.yml/badge.svg)](https://github.com/gourabsikder/SauceDemo.Playwright/actions/workflows/main.yml)

A professional UI automation testing framework for the [SauceDemo](https://www.saucedemo.com/) application, built using **C#, .NET 8, NUnit, Microsoft Playwright, and Page Object Model (POM)**.

The framework is designed with maintainability, reusability, configuration management, logging, failure diagnostics, and CI/CD execution in mind.

---

## 🚀 Tech Stack

- **Language:** C#
- **Framework:** .NET 8
- **Test Framework:** NUnit
- **UI Automation:** Microsoft Playwright
- **Design Pattern:** Page Object Model (POM)
- **Configuration:** Microsoft.Extensions.Configuration
- **Logging:** Custom Test Logger
- **Version Control:** Git / GitHub
- **CI/CD:** GitHub Actions
- **Browsers:** Chromium, Firefox, WebKit

---

## ✨ Framework Features

- Page Object Model architecture
- Reusable base test setup and teardown
- NUnit test execution
- Playwright browser automation
- Configurable browser selection
- Headless/headed browser configuration
- Centralized application configuration
- Environment-specific configuration support
- Environment variable overrides
- Structured test logging
- Automatic failure screenshots
- Playwright trace capture for failed tests
- Test result generation using TRX
- GitHub Actions CI/CD integration
- Automatic test artifact upload
- Support for Chromium, Firefox, and WebKit

---

## 🧪 Test Coverage

The current test suite contains **8 automated tests** covering the following areas:

| Area | Tests |
|---|---:|
| Login | 3 |
| Inventory | 2 |
| Cart | 2 |
| Checkout | 1 |
| **Total** | **8** |

### Scenarios Covered

#### Login
- Successful login
- Invalid credentials
- Locked-out user

#### Inventory
- Inventory page validation
- Product-related validation

#### Cart
- Adding products to cart
- Cart validation

#### Checkout
- Checkout workflow validation

---

## 🏗️ Project Structure

```text
SauceDemo.Playwright/
│
├── Base/
│   └── BaseTest.cs
│
├── Configuration/
│   ├── TestConfiguration.cs
│   └── appsettings.json
│
├── Logging/
│   └── TestLogger.cs
│
├── Pages/
│   ├── LoginPage.cs
│   ├── InventoryPage.cs
│   ├── CartPage.cs
│   └── CheckoutPage.cs
│
├── Tests/
│   ├── LoginTests.cs
│   ├── InventoryTests.cs
│   ├── CartTests.cs
│   └── CheckoutTests.cs
│
├── TestResults/
│   └── Artifacts/
│
├── .github/
│   └── workflows/
│       └── main.yml
│
├── .gitignore
├── README.md
└── SauceDemo.Playwright.slnx