// copyright (c) microsoft corporation. all rights reserved.
// licensed under the mit license.


using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Dynamics365.UIAutomation.Sample;
using Microsoft.Dynamics365.UIAutomation.Sample.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;




namespace Microsoft.Dynamics365.UIautomation.Sample
{


    [TestClass]
    public class CreateLead : TestsBase

    {
        

        [TestCategory("Entity")]
        [TestMethod]
        public void TestCreateLeadEndToEndFlow()




        //public TestContext testcontext { get; set; }

        //// csv path (relative to test project output or absolute)
        //private readonly string _csvpath = "testdata/leads.csv"; // ensure this file is copied to output (set copy to output directory = always)


        //[TestInitialize]
        
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))

            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);
                xrmApp.ThinkTime(5000);

                [TestCleanup]
                void Cleanup()
                {
                    client?.Dispose();
                }

                /// <summary>
                /// Full end-to-end flow: Create Lead → Qualify → Opportunity → Develop → Propose → Close Won.
                /// Driven by CSV data.
                /// </summary>
                [DataTestMethod]
                [DynamicData(nameof(GetLeadData), DynamicDataSourceType.Method)]
                 void Lead_To_Opportunity_ClosedAsWon(
                    string topic,
                    string firstName,
                    string lastName,
                    string company,
                    string mobile,
                    string email,
                    string revenue)
                {
                    // -----------------------------
                    // Step 1: Create Lead
                    // -----------------------------
                    xrmApp.Navigation.OpenSubArea("Sales", "Leads");
                    xrmApp.CommandBar.ClickCommand("New");

                    xrmApp.Entity.SetValue("subject", topic);
                    xrmApp.Entity.SetValue("firstname", firstName);
                    xrmApp.Entity.SetValue("lastname", lastName);
                    xrmApp.Entity.SetValue("companyname", company);
                    xrmApp.Entity.SetValue("mobilephone", mobile);
                    xrmApp.Entity.SetValue("emailaddress1", email);
                    xrmApp.CommandBar.ClickCommand("Save");

                    var leadName = xrmApp.Entity.GetValue("lastname");
                    Assert.AreEqual(lastName, leadName, "Lead creation failed.");

                    // -----------------------------
                    // Step 2: Qualify Lead → Opportunity
                    // -----------------------------
                    xrmApp.CommandBar.ClickCommand("Qualify");
                    xrmApp.ThinkTime(5000);

                    var oppTitle = xrmApp.Entity.GetHeaderTitle();
                    Assert.IsTrue(oppTitle.Contains("Opportunity"), "Lead was not qualified to Opportunity.");

                    // -----------------------------
                    // Step 3: Progress to Develop
                    // -----------------------------
                    xrmApp.BusinessProcessFlow.NextStage("Develop");
                    var activeStage = xrmApp.BusinessProcessFlow.GetActiveStage();
                    Assert.IsTrue(activeStage.Contains("Develop"), "Failed to move to Develop stage.");

                    // -----------------------------
                    // Step 4: Progress to Propose
                    // -----------------------------
                    xrmApp.BusinessProcessFlow.NextStage("Propose");
                    activeStage = xrmApp.BusinessProcessFlow.GetActiveStage();
                    Assert.IsTrue(activeStage.Contains("Propose"), "Failed to move to Propose stage.");

                    // -----------------------------
                    // Step 5: Close as Won
                    // -----------------------------
                    xrmApp.CommandBar.ClickCommand("Close as Won");
                    xrmApp.Dialogs.SetValue("actualrevenue", revenue);
                    xrmApp.Dialogs.SetValue("actualclosedate", DateTime.Today.ToString("MM/dd/yyyy"));
                    xrmApp.Dialogs.ClickCommand("OK");

                    xrmApp.ThinkTime(5000);

                    var status = xrmApp.Entity.GetValue("statuscode");
                    Assert.AreEqual("Won", status, "Opportunity was not closed as Won.");
                }
            }
        }

        /// <summary>
        /// Loads lead data from CSV for data-driven test execution.
        /// </summary>
        public static System.Collections.Generic.IEnumerable<object[]> GetLeadData()
        {
            var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Leads.csv");
            return CsvDataReader.GetTestData(csvPath);
        }
    }
}
