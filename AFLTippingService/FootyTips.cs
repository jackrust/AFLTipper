using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium;

namespace AFLTippingService
{
    public class FootyTips
    {
        private ISelenium selenium;
        private StringBuilder verificationErrors;

        public void SetupTest()
        {
            selenium = new DefaultSelenium("192.168.0.2", 4444, "*firefox", "http://www.google.com/");
            selenium.Start();
            verificationErrors = new StringBuilder();
        }

        public void TeardownTest()
        {
            try
            {
                selenium.Stop();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        public void TheNewTest()
        {
            selenium.Open("/");
            selenium.Type("q", "selenium rc");
            selenium.Click("btnG");
            selenium.WaitForPageToLoad("30000");
            Console.WriteLine("selenium rc - Google Search" + "/Vs/" + selenium.GetTitle());
        }
    }
}