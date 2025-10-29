using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection;
using SeleniumKeys = OpenQA.Selenium.Keys;

namespace Microsoft.Dynamics365.UIAutomation.Sample.UI.ApiTests
{
    [TestClass]
    public class CreateLead_End2EndFlow : TestsBase
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

        //[TestMethod]
        //[TestCategory("EndToEnd")]
        //public void LeadToOpportunity_ClosureAsWon()
        //{
        //    try
        //    {
        //        // ✅ Step 1: Navigate to Sales > Leads
        //        SafeAction(() =>
        //        {
        //            xrmApp.Navigation.OpenSubArea("Sales", "Leads");
        //            xrmApp.ThinkTime(5000);
        //        }, "Navigate to Leads");

        //        // ✅ Step 2: Create new Lead
        //        SafeAction(() =>
        //        {
        //            xrmApp.CommandBar.ClickCommand("New");
        //            xrmApp.ThinkTime(2000);

        //            xrmApp.Entity.SetValue("subject", "Automation Lead1");
        //            xrmApp.Entity.SetValue("firstname", "Radhika1");
        //            xrmApp.Entity.SetValue("lastname", "Veeravalli1");
        //            xrmApp.Entity.SetValue("emailaddress1", "radhika1@example.com");

        //            xrmApp.CommandBar.ClickCommand("Save");
        //            xrmApp.ThinkTime(4000);
        //        }, "Create and Save Lead");

        //        // ✅ Step 3: Qualify the Lead → Opportunity
        //        SafeAction(() =>
        //        {
        //            xrmApp.CommandBar.ClickCommand("Qualify");
        //            xrmApp.ThinkTime(8000);
        //        }, "Qualify Lead");

        //        // ✅ Step 4: Move through BPF stages
        //        MoveToNextStage("Develop");
        //        MoveToNextStage("Propose");
        //        MoveToNextStage("Close");

        //        // ✅ Step 5: Close as Won
        //        SafeAction(() =>
        //        {
        //            xrmApp.CommandBar.ClickCommand("Close as Won");
        //            xrmApp.ThinkTime(4000);

        //            // ✅ Fill "Close Opportunity" dialog
        //            xrmApp.Dialogs.SetValue(new OptionSet { Name = "statuscode", Value = "Won" });
        //            xrmApp.Dialogs.SetValue("actualrevenue", "100000");
        //            xrmApp.Dialogs.SetValue(new DateTimeControl("closedate") { Value = DateTime.Now });
        //            xrmApp.Dialogs.SetValue("description", "Opportunity closed successfully via automation test.");

        //            xrmApp.Dialogs.ClickCommand("OK");
        //            xrmApp.ThinkTime(5000);
        //        }, "Close Opportunity as Won");

        //        // ✅ Step 6: Verify Opportunity is marked as Won
        //        SafeAction(() =>
        //        {
        //            var status = xrmApp.Entity.GetValue(new OptionSet { Name = "statecode" });
        //            Assert.AreEqual("Won", status, "Opportunity not marked as Won.");
        //        }, "Validate Opportunity Status");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail($"Test failed with exception: {ex.Message}");
        //    }
        //}

        ///// <summary>
        ///// Safely performs an action with context and retry handling.
        ///// </summary>
        //private void SafeAction(Action action, string description = "")
        //{
        //    try
        //    {
        //        action.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = string.IsNullOrEmpty(description)
        //            ? $"Action failed: {ex.Message}"
        //            : $"{description} failed: {ex.Message}";
        //        Assert.Fail(message);
        //    }
        //}

        ///// <summary>
        ///// Moves the Business Process Flow (BPF) to the specified stage with retry logic and safe handling.
        ///// </summary>
        //private void MoveToNextStage(string stageName, int maxAttempts = 3)
        //{
        //    int attempt = 0;
        //    bool moved = false;

        //    while (attempt < maxAttempts && !moved)
        //    {
        //        try
        //        {
        //            attempt++;
        //            Console.WriteLine($"➡️ Attempting to move to stage '{stageName}' (Attempt {attempt}/{maxAttempts})");

        //            // Instead of reading current stage, just wait for BPF header
        //            Console.WriteLine("⏳ Waiting for Business Process Flow to load...");
        //            xrmApp.ThinkTime(8000);

        //            xrmApp.BusinessProcessFlow.NextStage(stageName);
        //            xrmApp.BusinessProcessFlow.SetActive(stageName);
        //            xrmApp.ThinkTime(4000);

        //            Console.WriteLine($"✅ Successfully moved to stage '{stageName}'.");
        //            moved = true;

        //        }

        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"⚠️ Attempt {attempt} failed for stage '{stageName}': {ex.Message}");
        //            xrmApp.ThinkTime(5000);

        //            if (attempt == maxAttempts)
        //            {
        //                Assert.Fail($"❌ Failed to move to stage '{stageName}' after {maxAttempts} attempts: {ex.Message}");
        //            }
        //        }
        //    }
        //}

        //private IWebDriver GetDriverFromClient(WebClient client)
        //{
        //    var browserType = client.Browser.GetType();
        //    var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
        //    return (IWebDriver)driverField?.GetValue(client.Browser);
        //}

        //private void MarkAllStageTasksComplete(IWebDriver driver)
        //{
        //    var checkboxes = driver.FindElements(By.XPath("//input[@type='checkbox' and not(@disabled)]"));
        //    foreach (var checkbox in checkboxes)
        //    {
        //        if (!checkbox.Selected)
        //        {
        //            checkbox.Click();
        //            Thread.Sleep(500);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Marks all incomplete BPF tasks as complete if required before finishing.
        ///// </summary>
        //private void MarkAllStageTasksComplete()
        //{
        //    string activeStageHeader = string.Empty;
        //    try
        //    {
        //        // ✅ Access the Selenium WebDriver through WebClient
        //        // ✅ Universal, works on older and newer EasyRepro versions
        //        var browserType = client.Browser.GetType();
        //        var driverField = browserType.GetField("Driver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //        var driver = (OpenQA.Selenium.IWebDriver)driverField?.GetValue(client.Browser);


        //        // ✅ Find all "Mark complete" dropdowns
        //        var dropdowns = driver.FindElements(By.XPath("//select[contains(@aria-label,'Mark complete')]"));

        //        foreach (var dropdown in dropdowns)
        //        {
        //            try
        //            {
        //                var select = new OpenQA.Selenium.Support.UI.SelectElement(dropdown);
        //                var options = select.Options.Select(o => o.Text.Trim()).ToList();

        //                if (options.Contains("Yes"))
        //                    select.SelectByText("Yes");
        //                else if (options.Contains("Completed"))
        //                    select.SelectByText("Completed");
        //                else
        //                    dropdown.SendKeys(OpenQA.Selenium.Keys.Enter);

        //                xrmApp.ThinkTime(500);
        //            }
        //            catch
        //            {
        //                continue; // Skip problematic dropdowns safely
        //            }
        //        }
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        // Ignore if not found
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"⚠️ Unable to mark tasks complete: {ex.Message}");
        //    }
        //}

        //[ClassCleanup]
        //public static void Cleanup()
        //{
        //    try
        //    {
        //        var browserType = client.Browser.GetType();

        //        // Try GetWebDriver() first
        //        var getWebDriver = browserType.GetMethod("GetWebDriver", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        //        IWebDriver driver = null;

        //        if (getWebDriver != null)
        //            driver = (IWebDriver)getWebDriver.Invoke(client.Browser, null);
        //        else
        //        {
        //            // Fallback for older builds (Driver is private)
        //            var driverField = browserType.GetField("Driver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //            driver = (IWebDriver)driverField?.GetValue(client.Browser);
        //        }

        //        driver?.Quit();
        //        Console.WriteLine("✅ Browser closed successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"⚠️ Cleanup failed: {ex.Message}");
        //    }
        //}


        [TestMethod]
        [TestCategory("EndToEnd")]
        public void LeadToOpportunity_ClosureAsWon_Refactored()
        {
            try
            {
                // Step 1: Navigate to Leads
                SafeAction(() =>
                {
                    xrmApp.Navigation.OpenSubArea("Sales", "Leads");
                    xrmApp.ThinkTime(5000);
                }, "Navigate to Leads");

                // Step 2: Create Lead
                SafeAction(() =>
                {
                    xrmApp.CommandBar.ClickCommand("New");
                    xrmApp.ThinkTime(2000);

                    xrmApp.Entity.SetValue("subject", "Automation Lead1");
                    xrmApp.Entity.SetValue("firstname", "Radhika1");
                    xrmApp.Entity.SetValue("lastname", "Veeravalli1");
                    xrmApp.Entity.SetValue("emailaddress1", "radhika1@example.com");

                    xrmApp.CommandBar.ClickCommand("Save");
                    xrmApp.ThinkTime(4000);
                }, "Create Lead");

                // Step 3: Qualify Lead → Opportunity
                SafeAction(() =>
                {
                    xrmApp.CommandBar.ClickCommand("Qualify");
                    xrmApp.ThinkTime(8000);
                }, "Qualify Lead");

                // Step 4: Move through BPF Stages (Develop → Propose → Close)
                MoveToNextStage("Develop");
                MoveToNextStage("Propose");
                MoveToNextStage("Close");

                // Step 5: Mark all tasks complete + Finish
                SafeAction(() =>
                {
                    var driver = GetWebDriver(client);
                    MarkAllStageTasksComplete(driver);

                    var finishBtn = driver.FindElement(By.XPath("//button[contains(@aria-label,'Finish')]"));
                    finishBtn.Click();
                    xrmApp.ThinkTime(4000);

                    driver.WaitUntilAvailable(By.XPath("//button[contains(@aria-label,'Finished')]"), TimeSpan.FromSeconds(10));
                    Console.WriteLine("✅ Stage marked as Finished.");
                }, "Finish Final Stage");

                // Step 6: Close as Won
                SafeAction(() =>
                {
                    xrmApp.CommandBar.ClickCommand("Close as Won");
                    xrmApp.ThinkTime(5000);

                    xrmApp.Dialogs.SetValue(new OptionSet { Name = "statuscode", Value = "Won" });
                    xrmApp.Dialogs.SetValue("actualrevenue", "10000");
                    xrmApp.Dialogs.SetValue(new DateTimeControl("closedate") { Value = DateTime.Now });
                    xrmApp.Dialogs.SetValue("description", "Closed as Won via refactored automation flow.");
                    xrmApp.Dialogs.ClickCommand("OK");

                    xrmApp.ThinkTime(5000);
                }, "Close Opportunity as Won");

                // Step 7: Validate Opportunity = Won
                SafeAction(() =>
                {
                    var status = xrmApp.Entity.GetValue(new OptionSet { Name = "statecode" });
                    Assert.AreEqual("Won", status, "❌ Opportunity not marked as Won.");
                    Console.WriteLine("✅ Verified Opportunity Status = Won.");
                }, "Validate Opportunity Status");
            }
            catch (Exception ex)
            {
                Assert.Fail($"❌ Test failed with exception: {ex.Message}");
            }
        }

        // ---------------- Helper Methods ----------------

        //private void MoveToNextStage(string stageName, int maxAttempts = 3)
        //{
        //    int attempt = 0;
        //    bool moved = false;

        //    while (attempt < maxAttempts && !moved)
        //    {
        //        try
        //        {
        //            attempt++;
        //            Console.WriteLine($"➡️ Moving to stage '{stageName}' (Attempt {attempt}/{maxAttempts})");
        //            xrmApp.ThinkTime(6000);

        //            xrmApp.BusinessProcessFlow.NextStage(stageName);
        //            xrmApp.BusinessProcessFlow.SetActive(stageName);
        //            xrmApp.ThinkTime(4000);

        //            // Click Next Stage button explicitly
        //            var driver = GetWebDriver(client);
        //            var nextStageBtn = driver.FindElement(By.XPath("//button[contains(@aria-label,'Next Stage')]"));
        //            nextStageBtn.Click();

        //            xrmApp.ThinkTime(4000);
        //            Console.WriteLine($"✅ Successfully moved to stage '{stageName}'.");
        //            moved = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"⚠️ Attempt {attempt} failed for '{stageName}': {ex.Message}");
        //            xrmApp.ThinkTime(4000);
        //            if (attempt == maxAttempts)
        //                Assert.Fail($"❌ Could not move to stage '{stageName}' after {maxAttempts} attempts.");
        //        }
        //    }
        //}

        private void MoveToNextStage(string stageName, int maxAttempts = 3)
        {
            int attempt = 0;
            bool moved = false;
            //var driver = xrmApp.Browser.Driver;
            //var driver = client.Browser.Driver;
            var driver = GetWebDriver(client);

            while (attempt < maxAttempts && !moved)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"➡️ Moving to stage '{stageName}' (Attempt {attempt}/{maxAttempts})");

                    // Small buffer wait for Dynamics UI updates
                    //xrmApp.ThinkTime(4000);

                    //// ✅ Ensure the Business Process Flow (BPF) is visible
                    //if (!xrmApp.BusinessProcessFlow.IsVisible)
                    //{
                    //    xrmApp.BusinessProcessFlow.Show();
                    //    xrmApp.ThinkTime(2000);
                    //}

                    xrmApp.ThinkTime(4000);

                    // ✅ Ensure the Business Process Flow (BPF) is visible
                    driver = GetWebDriver(client);
                    //var driver = client.Browser.Driver;
                    //var driver = xrmApp.Browser.Driver;
                    var bpfVisible = driver.FindElements(By.XPath("//div[contains(@id,'processHeaderStage')]")).Any();

                   
                    if (!bpfVisible)
                    {
                        Console.WriteLine("BPF not visible — waiting for it to load...");
                        xrmApp.ThinkTime(4000);

                        // Recheck visibility
                        bpfVisible = driver.FindElements(By.XPath("//div[contains(@id,'processHeaderStage')]")).Any();
                        if (!bpfVisible)
                            Console.WriteLine("⚠️ Warning: BPF still not visible — continuing anyway...");
                    }

                    if (!bpfVisible)
                    {
                        Console.WriteLine("BPF not visible — attempting to show it.");
                        try
                        {
                            driver = GetWebDriver(client); // Use your helper from earlier
                            var bpfHeader = driver.FindElements(By.XPath("//div[contains(@id,'processHeaderStage')]")).FirstOrDefault();

                            if (bpfHeader == null)
                            {
                                Console.WriteLine("Waiting for Business Process Flow to become visible...");
                                xrmApp.ThinkTime(4000);
                            }
                            else
                            {
                                Console.WriteLine("✅ Business Process Flow is visible.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Could not verify Business Process Flow visibility: {ex.Message}");
                        }
                        catch
                        {
                            Console.WriteLine("⚠️ Could not explicitly show BPF (likely already loaded). Continuing...");
                        }
                    }

                    EnsureBPFVisible();

                    // ✅ Select target stage
                    xrmApp.BusinessProcessFlow.SelectStage(stageName);
                    xrmApp.ThinkTime(2000);

                    // ✅ Handle the “Set Active” button (Qualify → Develop)
                    var setActiveButton = driver.FindElements(By.XPath("//button[contains(@data-id,'setActiveButton')]"))
                                                .FirstOrDefault();

                    if (setActiveButton != null && setActiveButton.Displayed)
                    {
                        Console.WriteLine($"🟢 Clicking 'Set Active' for stage '{stageName}'...");
                        setActiveButton.Click();
                        xrmApp.ThinkTime(3000);
                    }

                    // ✅ After activation, handle the “Next Stage” or “Finish” button
                    var nextOrFinishButton = driver.FindElements(By.XPath(
                        "//button[contains(@data-id,'nextButtonContainer')] | //button[contains(@data-id,'finishButton')]"
                    )).FirstOrDefault();

                    if (nextOrFinishButton != null && nextOrFinishButton.Displayed)
                    {
                        Console.WriteLine($"➡️ Clicking '{nextOrFinishButton.Text}' for stage '{stageName}'...");
                        nextOrFinishButton.Click();
                        xrmApp.ThinkTime(4000);
                    }
                    else
                    {
                        throw new Exception($"No 'Next Stage' or 'Finish' button found for stage '{stageName}'.");
                    }

                    Console.WriteLine($"✅ Successfully moved to stage '{stageName}'.");
                    moved = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Attempt {attempt} failed for '{stageName}': {ex.Message}");
                    xrmApp.ThinkTime(3000);

                    if (attempt == maxAttempts)
                    {
                        Assert.Fail($"❌ Could not move to stage '{stageName}' after {maxAttempts} attempts. Error: {ex.Message}");
                    }
                }
            }
        }


        private IWebDriver GetWebDriver(WebClient client)
        {
            var browserField = typeof(WebClient)
                .GetField("_browser", BindingFlags.NonPublic | BindingFlags.Instance);
            var browser = browserField?.GetValue(client);

            var driverField = browser?.GetType()
                .GetField("_driver", BindingFlags.NonPublic | BindingFlags.Instance);
            var driver = driverField?.GetValue(browser) as IWebDriver;

            if (driver == null)
                throw new InvalidOperationException("Unable to access WebDriver instance from WebClient.");

            return driver;
        }
        private void EnsureBPFVisible()
        {
            var driver = GetWebDriver(client);

           // var driver = xrmApp.Browser.Driver;
            var bpfVisible = driver.FindElements(By.XPath("//div[contains(@id,'processHeaderStage')]")).Any();

            if (!bpfVisible)
            {
                Console.WriteLine("⚙️ Waiting for BPF to become visible...");
                xrmApp.ThinkTime(4000);

                bpfVisible = driver.FindElements(By.XPath("//div[contains(@id,'processHeaderStage')]")).Any();
                if (!bpfVisible)
                    Console.WriteLine("⚠️ BPF not visible after wait. Proceeding anyway...");
            }
        }


        private void SafeAction(Action action, string description = "")
        {
            try { action.Invoke(); }
            catch (Exception ex)
            {
                var msg = string.IsNullOrEmpty(description)
                    ? $"Action failed: {ex.Message}"
                    : $"{description} failed: {ex.Message}";
                Assert.Fail(msg);
            }
        }

        //private IWebDriver GetWebDriver(WebClient client)
        //{
        //    var browserType = client.Browser.GetType();
        //    var driverField = browserType.GetField("Driver", BindingFlags.NonPublic | BindingFlags.Instance);
        //    return (IWebDriver)driverField?.GetValue(client.Browser);
        //}

        private void MarkAllStageTasksComplete(IWebDriver driver)
        {
            try
            {
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
                            //dropdown.SendKeys(Keys.Enter);
                        dropdown.SendKeys(SeleniumKeys.Enter);

                        Thread.Sleep(500);
                    }
                    catch { /* Skip invalid dropdowns */ }
                }
                Console.WriteLine("✅ All stage tasks marked complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Unable to mark tasks complete: {ex.Message}");
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
