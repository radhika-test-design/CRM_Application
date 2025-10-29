// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Dynamics365.UIAutomation.Sample.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using SeleniumKeys = OpenQA.Selenium.Keys;

namespace Microsoft.Dynamics365.UIAutomation.Sample.UI.ApiTests
{
    [TestClass]
    public class CreateLead_EndToEndFlow : TestsBase
    {
        private static XrmApp xrmApp;
        private static WebClient client;
        private const int MaxRetryCount = 3;
        private const int WaitTime = 3000;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            client = new WebClient(TestSettings.Options);
            xrmApp = new XrmApp(client);

            // Login to CRM
            xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);
            xrmApp.ThinkTime(5000);
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void LeadToOpportunity_ClosureAsWon()
        {
            try
            {
                // ✅ Step 1: Navigate to Leads
                SafeAction(() =>
                {
                    xrmApp.Navigation.OpenSubArea("Sales", "Leads");
                    xrmApp.ThinkTime(3000);
                }, "Navigate to Leads");

                // ✅ Step 2: Create new Lead
                SafeAction(() =>
                {
                    xrmApp.CommandBar.ClickCommand("New");
                    xrmApp.ThinkTime(2000);

                    xrmApp.Entity.SetValue("subject", "Automation Lead");
                    xrmApp.Entity.SetValue("firstname", "Radhika");
                    xrmApp.Entity.SetValue("lastname", "Veeravalli");
                    xrmApp.Entity.SetValue("emailaddress1", "radhika.veeravalli@example.com");

                    xrmApp.CommandBar.ClickCommand("Save");
                    xrmApp.ThinkTime(4000);
                }, "Create new Lead");

                // ✅ Step 3: Qualify the Lead
                SafeAction(() =>
                {
                    xrmApp.CommandBar.ClickCommand("Qualify");
                    xrmApp.ThinkTime(8000);
                }, "Qualify Lead");

                // ✅ Step 4: Handle "Set Active" (appears after Qualify and before Develop)
                SafeAction(() =>
                {
                    Console.WriteLine("🔎 Checking for 'Set Active' button after Qualify...");

                    var driver = GetWebDriver(client);
                    if (driver == null)
                    {
                        Console.WriteLine("⚠️ WebDriver is null — attempting to reattach to browser window...");
                        var browserType = client.Browser.GetType();
                        var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
                        driver = (IWebDriver)driverField?.GetValue(client.Browser);

                        if (driver == null)
                            throw new NullReferenceException("WebDriver could not be initialized after Qualify.");
                    }

                    // Wait for the new Opportunity form to load
                    xrmApp.ThinkTime(5000);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    wait.Until(d => d.FindElements(By.XPath("//div[contains(@data-id,'processHeader')]")).Any());

                    // Optional: handle if CRM opened a new tab/window
                    try
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    catch { /* ignore if single window */ }

                    // Look for "Set Active"
                    var setActiveButtons = driver.FindElements(By.XPath("//button[contains(@aria-label,'Set Active')]"));
                    if (setActiveButtons.Any())
                    {
                        Console.WriteLine("🟢 'Set Active' button found — clicking...");
                        setActiveButtons.First().Click();
                        xrmApp.ThinkTime(5000);

                        // Wait for button to disappear
                        wait.Until(d => !d.FindElements(By.XPath("//button[contains(@aria-label,'Set Active')]")).Any());
                        Console.WriteLine("✅ 'Set Active' clicked successfully — now ready for Develop stage.");
                    }
                    else
                    {
                        Console.WriteLine("ℹ️ No 'Set Active' button found — continuing to Develop stage.");
                    }
                }, "Set Active before Develop");

                // ✅ Step 5: Progress through Opportunity stages (Develop → Propose → Close)
                MoveToNextStage("Develop");
                MoveToNextStage("Propose");
                MoveToNextStage("Close");

                // ✅ Step 6: Mark all “Mark complete” fields and click Finish
                SafeAction(() =>
                {
                    var driver = GetWebDriver(client);
                    if (driver == null) throw new NullReferenceException("WebDriver is null while finishing stage.");

                    var dropdowns = driver.FindElements(By.XPath("//select[contains(@aria-label,'Mark complete')]"));
                    foreach (var dropdown in dropdowns)
                    {
                        try
                        {
                            var select = new SelectElement(dropdown);
                            var options = select.Options.Select(o => o.Text.Trim()).ToList();

                            if (options.Contains("Yes"))
                                select.SelectByText("Yes");
                            else if (options.Contains("Completed"))
                                select.SelectByText("Completed");
                            else
                                dropdown.SendKeys(SeleniumKeys.Enter);

                            Thread.Sleep(500);
                        }
                        catch { /* Continue even if one fails */ }
                    }

                    // Click Finish
                    Console.WriteLine("Clicking 'Finish' button...");
                    var finishBtn = driver.FindElements(By.XPath("//button[contains(@aria-label,'Finish')]")).FirstOrDefault();
                    finishBtn?.Click();
                    xrmApp.ThinkTime(4000);

                    // Wait for Finished confirmation
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElements(By.XPath("//button[contains(@aria-label,'Finished')]")).Any());
                    Console.WriteLine("✅ Stage marked as Finished.");
                }, "Complete Final Stage and Finish");

                // ✅ Step 7: Close as Won
                SafeAction(() =>
                {
                    Console.WriteLine("Clicking 'Close as Won'...");
                    xrmApp.CommandBar.ClickCommand("Close as Won");
                    xrmApp.ThinkTime(5000);

                    // Fill Close Opportunity dialog
                    xrmApp.Dialogs.SetValue(new OptionSet { Name = "statuscode", Value = "Won" });
                    xrmApp.Dialogs.SetValue("actualrevenue", "100000");
                    xrmApp.Dialogs.SetValue(new DateTimeControl("closedate") { Value = DateTime.Now });
                    xrmApp.Dialogs.SetValue("description", "Opportunity closed successfully via automation test.");

                    xrmApp.Dialogs.ClickCommand("OK");
                    xrmApp.ThinkTime(5000);
                    Console.WriteLine("✅ Opportunity closed as Won.");
                }, "Close Opportunity as Won");

                // ✅ Step 8: Verify Opportunity Status = Won
                SafeAction(() =>
                {
                    var status = xrmApp.Entity.GetValue(new OptionSet { Name = "statecode" });
                    Assert.AreEqual("Won", status, "❌ Opportunity not marked as Won.");
                    Console.WriteLine("✅ Verified: Opportunity is marked as Won.");
                }, "Validate Opportunity Status");
            }
            catch (Exception ex)
            {
                Assert.Fail($"❌ Test failed with exception: {ex.Message}");
            }
        }

        // ✅ Safe execution wrapper
        private void SafeAction(Action action, string actionDescription = "")
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                var msg = string.IsNullOrEmpty(actionDescription)
                    ? $"Action failed with exception: {ex.Message}"
                    : $"{actionDescription} failed with exception: {ex.Message}";
                Assert.Fail(msg);
            }
        }

        // ✅ Moves to a specific stage
        private void MoveToNextStage(string stageName)
        {
            const int maxRetries = 3;
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    Console.WriteLine($"➡️ Moving to stage '{stageName}' (Attempt {attempt + 1}/{maxRetries})");
                    xrmApp.ThinkTime(3000);

                    xrmApp.BusinessProcessFlow.SelectStage(stageName);
                    xrmApp.ThinkTime(2000);

                    var driver = GetWebDriver(client);
                    if (driver == null)
                        throw new NullReferenceException("WebDriver is null during MoveToNextStage.");

                    var nextStageBtn = driver.FindElements(By.XPath("//button[contains(@aria-label,'Next Stage')]")).FirstOrDefault();

                    if (nextStageBtn != null)
                    {
                        Console.WriteLine($"➡️ Clicking 'Next Stage' to move to '{stageName}'...");
                        nextStageBtn.Click();
                        xrmApp.ThinkTime(4000);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ 'Next Stage' button not found — may already be active.");
                    }

                    Console.WriteLine($"✅ Successfully moved to stage '{stageName}'.");
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"⚠️ Attempt {attempt} failed for stage '{stageName}': {ex.Message}");
                    xrmApp.ThinkTime(2000);
                    if (attempt == maxRetries)
                        throw;
                }
            }
        }

        // ✅ Safe WebDriver accessor
        private IWebDriver GetWebDriver(WebClient client)
        {
            try
            {
                if (client == null || client.Browser == null)
                    return null;

                var browserType = client.Browser.GetType();
                var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
                var driver = (IWebDriver)driverField?.GetValue(client.Browser);

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

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                var browserType = client.Browser.GetType();
                var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
                var driver = (IWebDriver)driverField?.GetValue(client.Browser);
                driver?.Quit();
                Console.WriteLine("✅ Browser closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Cleanup failed: {ex.Message}");
            }
        }
    }
}
