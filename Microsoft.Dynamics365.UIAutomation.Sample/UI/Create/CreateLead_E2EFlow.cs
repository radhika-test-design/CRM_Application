//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT license.

//using Microsoft.Dynamics365.UIAutomation.Api;
//using Microsoft.Dynamics365.UIAutomation.Browser;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using System;
//using System.Security;
//using System.Linq;

//namespace Microsoft.Dynamics365.UIAutomation.Sample
//{

//    [TestClass]
//    public class CreateLead_E2EFlow : TestsBase

//    {


//        private XrmApp xrmApp;
//        private WebClient webClient;
//        private BrowserOptions browserOptions;

//        // CRM and profile configuration
//        private readonly string crmUrl = "https://org03d0bcf3.crm.dynamics.com/";
//        private readonly string userDataDir = @"C:\Users\<YOUR_USER>\AppData\Local\Google\Chrome\User Data";
//        private readonly string profileDirectory = "Profile 2"; // or "Default"
//        private readonly string username = "user@domain.com";
//        private readonly string password = "YourPassword";

//        [TestInitialize]
//        public void Setup()
//        {
//            // 1️⃣ Configure Chrome options to reuse existing logged-in profile
//            var chromeOptions = new ChromeOptions();
//            chromeOptions.AddArgument($"--user-data-dir={userDataDir}");
//            chromeOptions.AddArgument($"--profile-directory={profileDirectory}");
//            chromeOptions.AddArgument("--start-maximized");
//            chromeOptions.AddExcludedArgument("enable-automation");
//            //chromeOptions.AddAdditionalCapability("useAutomationExtension", false);

//            browserOptions = new BrowserOptions
//            {
//                BrowserType = BrowserType.Chrome,
//                ChromeOptions = chromeOptions
//            };

//            webClient = new WebClient(browserOptions);
//            xrmApp = new XrmApp(webClient);

//            // 2️⃣ Try opening Dynamics 365 directly
//            xrmApp.Navigation.OpenBrowser(new Uri(crmUrl));

//            // 3️⃣ Check if user is already logged in
//            if (IsLoginPage(webClient))
//            {
//                Console.WriteLine("⚠️ Session expired or login page detected — performing EasyRepro login...");
//                xrmApp.OnlineLogin.Login(crmUrl, _username, _password);
//            }
//            else
//            {
//                Console.WriteLine("✅ Active CRM session detected — skipping login.");
//            }

//            xrmApp.ThinkTime(2000);
//        }

//        [TestMethod]
//        public void Test_Open_SalesHub_HybridLogin()
//        {
//            // Open Sales Hub to verify session
//            xrmApp.Navigation.OpenApp("Sales Hub");
//            xrmApp.ThinkTime(3000);

//            Console.WriteLine("🎯 Opened Sales Hub successfully!");
//            Assert.IsTrue(true);
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            webClient?.Dispose();
//        }

//        /// <summary>
//        /// Checks if login page is displayed by inspecting URL or key elements.
//        /// </summary>
//        private bool IsLoginPage(WebClient client)
//        {
//            try
//            {
//                var driver = client.Browser.Driver;
//                var currentUrl = driver.Url.ToLower();

//                // If redirected to Microsoft login or login form visible
//                if (currentUrl.Contains("login.microsoftonline.com") ||
//                    currentUrl.Contains("signin") ||
//                    driver.Title.Contains("Sign in", StringComparison.OrdinalIgnoreCase))
//                {
//                    return true;
//                }

//                // Optional: check for username input field
//                bool hasEmailField = driver.FindElements(By.Id("i0116")).Any();
//                if (hasEmailField) return true;

//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("⚠️ Could not verify login state: " + ex.Message);
//                return true; // fallback to login
//            }
//        }
//    }
//}

