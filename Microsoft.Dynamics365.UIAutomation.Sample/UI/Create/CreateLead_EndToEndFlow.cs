// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Dynamics365.UIAutomation.Sample;
using Microsoft.Dynamics365.UIAutomation.Sample.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using SeleniumKeys = OpenQA.Selenium.Keys;

namespace Microsoft.Dynamics365.UIAutomation.Sample
{
    [TestClass]
    public class CreateLead_EndtoEndFlow : TestsBase
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
                SafeAction(() =>
                {
                    Console.WriteLine("➡️ Navigating to Leads...");
                    xrmApp.Navigation.OpenSubArea("Sales", "Leads");
                    xrmApp.ThinkTime(3000);
                }, "Navigate to Leads");

                SafeAction(() =>
                {
                    Console.WriteLine("🆕 Creating new Lead...");
                    xrmApp.CommandBar.ClickCommand("New");
                    xrmApp.ThinkTime(2000);

                    xrmApp.Entity.SetValue("subject", "Automation Lead - " + DateTime.Now.ToString("HHmmss"));
                    xrmApp.Entity.SetValue("firstname", "Test");
                    xrmApp.Entity.SetValue("lastname", "Lead");
                    xrmApp.Entity.SetValue("emailaddress1", "testlead@example.com");

                    xrmApp.CommandBar.ClickCommand("Save");
                    xrmApp.ThinkTime(4000);
                    Console.WriteLine("✅ Lead saved successfully.");
                }, "Create Lead");

                SafeAction(() =>
                {
                    Console.WriteLine("⚙️ Qualifying Lead...");
                    xrmApp.CommandBar.ClickCommand("Qualify");
                    xrmApp.ThinkTime(7000);
                    Console.WriteLine("✅ Lead qualified to Opportunity.");
                }, "Qualify Lead");

                MoveToNextStage("Develop");
                MoveToNextStage("Propose");
                CompleteCloseStage();
                ValidateOpportunityWon();
            }
            catch (Exception ex)
            {
                Assert.Fail($"❌ Test failed with exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Completes the Close stage: Finish → Finished → Close as Won → OK.
        /// Handles retries and dynamic popups.
        /// </summary>
        private void CompleteCloseStage()
        {
            SafeAction(() =>
            {
                Console.WriteLine("➡️ Starting Close stage automation...");

                var driver = GetWebDriver(client);
                if (driver == null)
                    throw new NullReferenceException("WebDriver not initialized for Close stage.");

                driver.SwitchTo().DefaultContent();

                // ===== Step 1: Click the Finish button =====
                var finishButton = driver.WaitUntilAvailable(By.XPath("//button[contains(@aria-label,'Finish') or @data-id='businessprocessflow-flyout-Finish']"), 10);
                if (finishButton == null) throw new Exception("❌ 'Finish' button not found.");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", finishButton);
                driver.TakeScreenshot("Before_Click_Finish");
                finishButton.Click();
                Console.WriteLine("✅ Clicked 'Finish' button.");

                // ===== Step 2: Click the Finished button =====
                var finishedButton = driver.WaitUntilAvailable(By.XPath("//button[contains(@aria-label,'Finished') or @data-id='businessprocessflow-flyout-Finished']"), 10);
                finishedButton.Click();
                driver.TakeScreenshot("After_Click_Finished");
                Console.WriteLine("✅ Clicked 'Finished' button.");

                // ===== Step 3: Click the 'Close as Won' button =====
                var closeAsWonButton = driver.WaitUntilAvailable(By.XPath("//button[contains(@aria-label,'Close as won') or @data-id='opportunity|NoRelationship|Form|Mscrm.Form.opportunity.CloseAsWon']"), 10);
                closeAsWonButton.Click();
                driver.TakeScreenshot("After_Click_CloseAsWon");
                Console.WriteLine("✅ Clicked 'Close as Won' button.");

                // ===== Step 4: Wait for the 'Close Opportunity' popup =====
                var revenueField = driver.WaitUntilVisible(By.XPath("//input[contains(@aria-label,'Actual Revenue') or contains(@id,'actualrevenue')]"), TimeSpan.FromSeconds(10));
                if (revenueField == null)
                    throw new Exception("❌ Actual Revenue input not found on Close Opportunity dialog.");

                // Fill Actual Revenue
                revenueField.Clear();
                revenueField.SendKeys("10000");
                Console.WriteLine("💰 Entered Actual Revenue: 10000");

                // ===== Step 5: Click OK =====
                var okButton = driver.WaitUntilAvailable(By.XPath("//button[@data-id='ok_id' or contains(@aria-label,'OK')]"), 10);
                okButton.Click();
                driver.TakeScreenshot("After_Click_OK");
                Console.WriteLine("✅ Clicked OK on Close Opportunity popup.");

                // ===== Step 6: Verify Status is 'Won' =====
                driver.WaitUntilVisible(By.XPath("//span[contains(text(),'Won')]"), TimeSpan.FromSeconds(10));
                driver.TakeScreenshot("RecordStatus_Won");
                Console.WriteLine("🎉 Opportunity successfully closed as WON.");

            }, "Complete Close Stage Flow");
        }



        /// <summary>
        /// Validates that the opportunity record status is "Won".
        /// </summary>
        private IWebDriver GetWebDriver(WebClient client)
        {
            if (client == null)
                throw new NullReferenceException("WebClient is null.");

            var browserType = client.Browser.GetType();

            // Try GetWebDriver() (modern builds)
            var getWebDriverMethod = browserType.GetMethod("GetWebDriver",
                BindingFlags.Public | BindingFlags.Instance);

            if (getWebDriverMethod != null)
            {
                var driver = getWebDriverMethod.Invoke(client.Browser, null) as IWebDriver;
                if (driver != null) return driver;
            }

            // Fallback: protected field (legacy builds)
            var driverField = browserType.GetField("Driver",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (IWebDriver)driverField?.GetValue(client.Browser);
        }

        private void MoveToNextStage(string stageName)
        {
            SafeAction(() =>
            {
                Console.WriteLine($"➡️ Moving to stage '{stageName}'...");
                xrmApp.BusinessProcessFlow.SelectStage(stageName);
                xrmApp.ThinkTime(1500);
                xrmApp.BusinessProcessFlow.NextStage(stageName);
                xrmApp.ThinkTime(3000);
                Console.WriteLine($"✅ Successfully moved to stage '{stageName}'.");
            }, $"Move to stage {stageName}");
        }

        private void ValidateOpportunityWon()
        {
            SafeAction(() =>
            {
                Console.WriteLine("🔍 Verifying Opportunity status...");
                var status = xrmApp.Entity.GetValue(new OptionSet { Name = "statecode" });
                Assert.AreEqual("Won", status, "❌ Opportunity not marked as Won.");
                Console.WriteLine("✅ Verified: Opportunity is marked as Won.");
            }, "Validate Opportunity Status");
        }

        private void CaptureScreenshot(IWebDriver driver, string fileName)
        {
            try
            {
                string folder = "C:\\TestResults";
                Directory.CreateDirectory(folder);

                string fullPath = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}");
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
                Console.WriteLine($"📸 Screenshot saved: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Screenshot capture failed: {ex.Message}");
            }
        }

        private void SafeAction(Action action, string description = "")
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                var msg = string.IsNullOrEmpty(description)
                    ? $"Action failed: {ex.Message}"
                    : $"{description} failed: {ex.Message}";
                Assert.Fail(msg);
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
