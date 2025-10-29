// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System;
using System.Security;
using OpenQA.Selenium.Chrome;

namespace Microsoft.Dynamics365.UIAutomation.Sample
{
    
    [TestClass]
    public class CreateLead : TestsBase
        
    {
        //private ChromeDriver driver = new ChromeDriver();

        [TestCategory("Entity")]
        [TestMethod]
        public void TestCreateLead()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                //xrmApp.Navigation.OpenApp(AppName.Sales);

                //string leadUrl = $"{TestSettings.XrmUri}/main.aspx?appid=appid=4c3b67a9-c58b-f011-b4cb-6045bd04a9b1&pagetype=control&controlName=MscrmControls.AcceleratedSales.AnchorShellControl";
                xrmApp.ThinkTime(5000);
               // xrmApp.Navigation.OpenApp("Sales Hub");
                //xrmApp.Navigation.OpenUrl(new Uri($"{TestSettings.XrmUri}/main.aspx?appid=appid=4c3b67a9-c58b-f011-b4cb-6045bd04a9b1&pagetype=control&controlName=MscrmControls.AcceleratedSales.AnchorShellControl"));
                
               // driver.Navigate().GoToUrl("https://org812e0186.crm.dynamics.com/main.aspx?appid=4c3b67a9-c58b-f011-b4cb-6045bd04a9b1&forceUCI=1&newWindow=true&pagetype=entitylist&etn=lead&viewid=00000000-0000-0000-00aa-000010001005&viewType=1039");
             


                
                BrowserCommandResult<bool> browserCommandResult = xrmApp.Navigation.OpenSubArea("Sales", "Leads");
                xrmApp.ThinkTime(5000);
                xrmApp.CommandBar.ClickCommand("New");
                xrmApp.ThinkTime(5000);



                xrmApp.Entity.SetValue("subject", TestSettings.GetRandomString(5,15));
                xrmApp.Entity.SetValue("firstname", TestSettings.GetRandomString(5,10));
                xrmApp.Entity.SetValue("lastname", TestSettings.GetRandomString(5,10));

                xrmApp.Entity.Save();

            }
            
        }
    }
}

