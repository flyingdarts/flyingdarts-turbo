# Flying Darts E2E Testing - POM Architecture 🎯

This directory contains the End-to-End (E2E) test suite for the Flying Darts application, built using the **Page Object Model (POM)** architecture with C# and Playwright.

## 🏗️ Architecture Overview

The POM architecture provides a structured approach to E2E testing by separating test logic from page interactions. This makes tests more maintainable, readable, and reusable.

### Core Components

```
e2e/
├── Pages/           # Page Object classes
├── Tests/           # Test classes
├── TestData/        # Test data and constants
├── Utilities/       # Helper classes and utilities
├── Configuration/   # Test configuration settings
└── README.md        # This documentation
```

## 📁 Directory Structure

### Pages/
Contains page object classes that represent web pages:
- **`BasePage.cs`** - Abstract base class with common functionality
- **`HomePage.cs`** - Home page interactions
- **`SettingsPage.cs`** - Game settings page interactions

### Tests/
Contains test classes that use page objects:
- **`BaseTest.cs`** - Abstract base class for all tests
- **`GameSettingsTests.cs`** - Example tests for game settings

### TestData/
Contains test data and constants:
- **`GameSettingsData.cs`** - Game settings configurations for testing

### Utilities/
Contains helper classes:
- **`AuthressHelper.cs`** - Authentication utility

### Configuration/
Contains configuration settings:
- **`TestConfiguration.cs`** - Environment-specific settings

## 🚀 Getting Started

### Prerequisites

1. **.NET 9.0** or later
2. **Playwright** installed: `playwright install`
3. **Environment Variables**:
   ```bash
   export AUTHRESS_SERVICE_CLIENT_ACCESS_KEY="your_access_key"
   export TEST_ENVIRONMENT="staging"  # Optional: staging, production, development, local
   ```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~GameSettingsTests"

# Run with specific environment
TEST_ENVIRONMENT=production dotnet test

# Run with custom timeout
TEST_TIMEOUT=60000 dotnet test
```

## 📝 Writing Tests

### 1. Create a Page Object

```csharp
public class LoginPage : BasePage
{
    private readonly ILocator _usernameInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;

    public LoginPage(IPage page, string baseUrl = "https://staging.flyingdarts.net") 
        : base(page, baseUrl)
    {
        _usernameInput = Page.GetByLabel("Username");
        _passwordInput = Page.GetByLabel("Password");
        _loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    }

    public async Task LoginAsync(string username, string password)
    {
        await WaitForElementVisibleAsync(_usernameInput);
        await FillWithRetryAsync(_usernameInput, username);
        await FillWithRetryAsync(_passwordInput, password);
        await ClickWithRetryAsync(_loginButton);
    }
}
```

### 2. Create a Test Class

```csharp
public class LoginTests : BaseTest
{
    [Fact]
    public async Task ShouldLoginSuccessfully()
    {
        // Arrange
        await SetupAsync();
        var loginPage = GetPage<LoginPage>();

        // Act
        await loginPage.NavigateToAsync("login");
        await loginPage.LoginAsync("testuser", "password");

        // Assert
        Assert.Contains("dashboard", Page.Url);
    }
}
```

## 🔧 Key Features

### BasePage Class
- **Navigation helpers** - Easy URL navigation
- **Element waiting** - Smart element waiting with timeouts
- **Retry logic** - Automatic retry for flaky operations
- **Screenshot support** - Debug screenshots
- **Error handling** - Graceful error handling

### BaseTest Class
- **Authentication setup** - Automatic Authress token handling
- **Page object factory** - Easy page object instantiation
- **Common assertions** - Reusable assertion methods
- **Setup/teardown** - Consistent test lifecycle

### Test Data Management
- **Centralized constants** - Easy to maintain test data
- **Environment-specific values** - Different configs per environment
- **Data-driven testing** - Support for parameterized tests

## 🌍 Environment Configuration

The framework supports multiple environments through environment variables:

```bash
# Environment selection
export TEST_ENVIRONMENT="staging"      # staging, production, development, local

# Timeout configuration
export TEST_TIMEOUT="60000"            # 60 seconds

# Screenshot settings
export TEST_SCREENSHOTS="true"         # Enable/disable screenshots
export TEST_SCREENSHOTS_DIR="debug"    # Custom screenshot directory

# Browser settings
export TEST_HEADLESS="false"           # Show browser during tests
```

## 📊 Best Practices

### 1. Page Object Design
- **Single responsibility** - Each page object handles one page
- **Encapsulation** - Hide implementation details from tests
- **Reusability** - Common methods in base classes
- **Maintainability** - Easy to update when UI changes

### 2. Test Design
- **Arrange-Act-Assert** - Clear test structure
- **Descriptive names** - Tests should read like documentation
- **Independent tests** - Tests should not depend on each other
- **Data-driven** - Use test data for multiple scenarios

### 3. Locator Strategy
- **Accessibility first** - Use `GetByRole` and `GetByLabel`
- **Text content** - Use `GetByText` for visible text
- **Avoid XPath** - Use Playwright's built-in locators
- **Stable selectors** - Avoid selectors that change frequently

### 4. Error Handling
- **Retry logic** - Handle flaky operations gracefully
- **Meaningful errors** - Clear error messages for debugging
- **Screenshots** - Capture state on failures
- **Logging** - Log important steps for debugging

## 🐛 Debugging

### Screenshots
Tests automatically take screenshots on failures and when requested:

```csharp
await TakeScreenshotAsync("debug_point");
```

### Headless Mode
Disable headless mode to see the browser during test execution:

```bash
export TEST_HEADLESS="false"
dotnet test
```

### Timeouts
Adjust timeouts for slower environments:

```bash
export TEST_TIMEOUT="60000"  # 60 seconds
dotnet test
```

## 🔄 Maintenance

### Updating Page Objects
When the UI changes:
1. Update locators in the affected page object
2. Update any hardcoded text or selectors
3. Run tests to ensure they still pass
4. Update documentation if needed

### Adding New Pages
1. Create a new page object class extending `BasePage`
2. Define locators for page elements
3. Implement page-specific methods
4. Create corresponding tests
5. Update this documentation

## 📚 Additional Resources

- [Playwright Documentation](https://playwright.dev/dotnet/)
- [xUnit Documentation](https://xunit.net/)
- [C# Best Practices](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 🤝 Contributing

When contributing to the E2E test suite:

1. **Follow the POM pattern** - Use existing page objects or create new ones
2. **Add tests for new features** - Ensure new functionality is tested
3. **Update documentation** - Keep this README current
4. **Use meaningful names** - Clear, descriptive names for methods and variables
5. **Handle errors gracefully** - Implement proper error handling and retry logic

---

Happy testing! 🎯✨
