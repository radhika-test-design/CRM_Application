using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics365.UIAutomation.Sample.Helpers
{
    public static class WebDriverExtensions
    {
        public static IWebElement WaitUntilAvailable(this IWebDriver driver, By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
        }

        public static IWebElement WaitUntilVisible(this IWebDriver driver, By by, TimeSpan? timeout = null)
        {
            var wait = new WebDriverWait(driver, timeout ?? TimeSpan.FromSeconds(10));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }

        public static void TakeScreenshot(this IWebDriver driver, string filename)
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var screenshotDir = Path.Combine(baseDir, "Downloads");

            if (!Directory.Exists(screenshotDir))
                Directory.CreateDirectory(screenshotDir);

            var path = Path.Combine(screenshotDir, $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            //var path = Path.Combine(TestSettings.DownloadDir, $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            screenshot.SaveAsFile(path, ScreenshotImageFormat.Png);
            Console.WriteLine($"📸 Screenshot saved: {path}");
        }
    }

}
