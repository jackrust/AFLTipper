using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Tests
{
    [TestClass]
    public class HtmlyTests
    {
        [TestMethod]
        public void ExtractElements_Should_ExtractContent()
        {
            var element = Htmly.ExtractElements("<div>Dinosaur<div/>");
            Assert.AreEqual("Dinosaur", element.Content);
        }

        [TestMethod]
        public void ExtractElements_Should_ExtractEmptyElement()
        {
            var element = Htmly.ExtractElements("<div><><div/>");
            Assert.AreEqual("Dinosaur", element.Content);
        }

        [TestMethod]
        public void FindMe_Should_FindDetailsInSafewayHTML()
        {
            //Arrange
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var html = Filey.Load(path + @"\..\..\TestDataSafewayHTML.txt");
            //HtmlySearchInformation searchInformation = new HtmlySearchInformation();
            //Act 
            var cost = Htmly.FindCostIn(html, "almonds");

            //Assert
            Assert.IsTrue(cost.Length > 0);

        }
    }
}
