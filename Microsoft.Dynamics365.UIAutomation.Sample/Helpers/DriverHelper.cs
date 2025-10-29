using OpenQA.Selenium;
using System.Reflection;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Dynamics365.UIAutomation.Api;

namespace Microsoft.Dynamics365.UIAutomation.Sample.Helpers
{
    /// <summary>
    /// Helper utility for safely retrieving Selenium WebDriver from EasyRepro WebClient.
    /// Works across all EasyRepro versions (modern and legacy).
    /// </summary>
    public static class DriverHelper
    {
        /// <summary>
        /// Returns the IWebDriver instance from a WebClient safely.
        /// Works for both modern (GetWebDriver) and legacy EasyRepro builds.
        /// </summary>
        //public static IWebDriver GetWebDriver(WebClient client)
        //{
        //    if (client?.Browser == null)
        //        throw new System.Exception("Browser client is not initialized.");

        //    // ✅ Modern EasyRepro versions (9.1+)
        //    var browserType = client.Browser.GetType();
        //    var getWebDriverMethod = browserType.GetMethod("GetWebDriver", BindingFlags.Public | BindingFlags.Instance);

        //    if (getWebDriverMethod != null)
        //    {
        //        var driver = getWebDriverMethod.Invoke(client.Browser, null) as IWebDriver;
        //        if (driver != null)
        //            return driver;
        //    }

        //    // ✅ Fallback for older builds (Driver is private/protected)
        //    var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (driverField != null)
        //    {
        //        var driver = driverField.GetValue(client.Browser) as IWebDriver;
        //        if (driver != null)
        //            return driver;
        //    }

        //    throw new System.Exception("Unable to access WebDriver from WebClient. Verify EasyRepro version compatibility.");
        //}

        //public static IWebDriver GetWebDriver(WebClient client)
        //{
        //    // Try to access the internal 'Browser' or '_browser' field
        //    var browserField = typeof(WebClient).GetField("_browser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //                       ?? typeof(WebClient).GetField("Browser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        //    var browser = browserField?.GetValue(client);
        //    if (browser == null)
        //        throw new InvalidOperationException("❌ Could not locate Browser object inside WebClient.");

        //    // Try all possible driver field names (_driver, driver)
        //    var driverField = browser.GetType().GetField("_driver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //                     ?? browser.GetType().GetField("driver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //                     ?? browser.GetType().GetField("Driver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        //    var driver = driverField?.GetValue(browser) as IWebDriver;

        //    if (driver == null)
        //        throw new InvalidOperationException("❌ Could not access WebDriver instance from EasyRepro WebClient.");

        //    return driver;
        //}

        /// <summary>
        /// Retrieves the internal Selenium WebDriver instance from EasyRepro’s WebClient
        /// using reflection. Compatible with all EasyRepro versions (2019–2025).
        /// </summary>
        //public static IWebDriver GetWebDriver(WebClient client)
        //{
        //    // Try to access internal Browser field (_browser or Browser)
        //    var browserField = typeof(WebClient).GetField("_browser", BindingFlags.NonPublic | BindingFlags.Instance)
        //                       ?? typeof(WebClient).GetField("Browser", BindingFlags.NonPublic | BindingFlags.Instance);

        //    var browser = browserField?.GetValue(client);
        //    if (browser == null)
        //        throw new InvalidOperationException("❌ Could not locate Browser object inside WebClient.");

        //    // Try to access the internal Selenium driver field (_driver, driver, or Driver)
        //    var driverField = browser.GetType().GetField("_driver", BindingFlags.NonPublic | BindingFlags.Instance)
        //                     ?? browser.GetType().GetField("driver", BindingFlags.NonPublic | BindingFlags.Instance)
        //                     ?? browser.GetType().GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);

        //    var driver = driverField?.GetValue(browser) as IWebDriver;

        //    if (driver == null)
        //        throw new InvalidOperationException("❌ Could not access WebDriver instance from EasyRepro WebClient.");

        //    return driver;
        //}

        public static IWebDriver GetWebDriver(WebClient client)
        {
            try
            {
                if (client == null || client.Browser == null)
                    return null;

                var browserType = client.Browser.GetType();
                var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
                var driver = (IWebDriver)driverField?.GetValue(client.Browser);

                // Additional sanity check
                if (driver == null)
                    Console.WriteLine("⚠️ WebDriver reference is null inside GetWebDriver.");

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to get WebDriver: {ex.Message}");
                return null;
            }
        }


    }
}
