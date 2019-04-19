using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Tests
{
    [TestClass]
    public class StringyTests
    {
        [TestMethod]
        public void ShouldReturnOneHundredCharacterSpacePaddedString()
        {
            //Arrange
            const string inString = "Just 10 characters long";
            const int requiredLength = 100;
            //Act
            string result = Stringy.Lengthen(inString, requiredLength);
            //assert
            Assert.AreEqual(100, result.Length);
            Assert.AreEqual(' ', result[99]);
        }

        [TestMethod]
        public void ShouldReturnOneForExactMatch()
        {
            //Arrange
            const string original = "string";
            //Act
            var result = Stringy.Compare(original, original);
            //Assert
            Assert.AreEqual(1, result);
        }
        [TestMethod]
        public void ShouldReturnNegativeOneForExactNull()
        {
            //Arrange
            const string original = "string";
            //Act
            var result = Stringy.Compare(original, null);
            //Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ShouldPreferExactMatchToPartialToNoMatchToBlank()
        {
            //Arrange
            const string original = "string";
            const string partial = "strizang";
            const string noMatch = "qweyuop";
            const string blank = "";
            //Act
            var exactResult = Stringy.Compare(original, original);
            var partialResult = Stringy.Compare(original, partial);
            var noMatchResult = Stringy.Compare(original, noMatch);
            var blankResult = Stringy.Compare(original, blank);
            //Assert
            Assert.IsTrue(exactResult > partialResult);
            Assert.IsTrue(partialResult > noMatchResult);
            Assert.AreEqual(0, noMatchResult);
            Assert.AreEqual(0, blankResult);
        }
    }
}
