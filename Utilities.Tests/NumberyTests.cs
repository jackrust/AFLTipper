using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Untilities.Tests
{
    [TestClass]
    public class NumberyTests
    {
        [TestMethod]
        public void TestNormaliser_For_Base_Rage()
        {
            Assert.AreEqual(0.5, Numbery.Normalise(75, new double[] { 25, 125 }, new double[] { 0, 1 }, Numbery.NormalisationMethod.Normal));
        }
        [TestMethod]
        public void TestNormaliser_For_Positive_Offset()
        {
            Assert.AreEqual(0.5, Numbery.Normalise(50, new double[] { 0, 100 }, new double[] { 0, 1 }, Numbery.NormalisationMethod.Normal));
        }
        [TestMethod]
        public void TestNormaliser_For_Negative_Offset()
        {
            Assert.AreEqual(0.5, Numbery.Normalise(25, new double[] { -25, 75 }, new double[] { 0, 1 }, Numbery.NormalisationMethod.Normal));
        }
        [TestMethod]
        public void TestNormaliser_For_Large_Result_Rage()
        {
            Assert.AreEqual(50, Numbery.Normalise(75, new double[] { 25, 125 }, new double[] { 0, 100 }, Numbery.NormalisationMethod.Normal));
        }
        [TestMethod]
        public void TestNormaliser_For_Positive_Offset_Result()
        {
            var result = Numbery.Normalise(50, new double[] {0, 100}, new double[] {100, 200},
                Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(150, result);
        }
        [TestMethod]
        public void TestNormaliser_For_Negative_Offset_Result()
        {
            var result = Numbery.Normalise(25, new double[] {-25, 75}, new double[] {-100, 0},
                Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(-50, result);
        }
        [TestMethod]
        public void TestNormaliser_For_Normalise()
        {
            var result = Numbery.Normalise(150, 200, Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(0.75, result);
        }
        [TestMethod]
        public void TestNormaliser_For_Deormalise()
        {
            var result = Numbery.Denormalise(0.75, 200, Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(150, result);
        }
        [TestMethod]
        public void TestNormaliser_For_Normalise_Offset()
        {
            var result = Numbery.Normalise(20, 10, 110, 2, 12, Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(3, result);
        }
        [TestMethod]
        public void TestNormaliser_For_Deormalise_Offset()
        {
            var result = Numbery.Denormalise(3, 2, 12, 10, 110, Numbery.NormalisationMethod.Normal);
            Assert.AreEqual(20, result);
        }


        [TestMethod]
        public void TestNormaliser_For_NormaliseGradiated()
        {
            var output = Numbery.Normalise(150, 200, Numbery.NormalisationMethod.Gradiated);

            Assert.AreEqual(0.75, output);
        }

        [TestMethod]
        public void TestDenormaliser_For_DenormaliseGradiated()
        {
            var output = Numbery.Denormalise(0.75, 200, Numbery.NormalisationMethod.Gradiated);

            Assert.AreEqual(150, output);
        }

        [TestMethod]
        public void TestNormaliser_For_NormaliseAsymptotic()
        {
            var output = Numbery.Normalise(36, 100, Numbery.NormalisationMethod.Asymptotic);

            Assert.IsTrue(output > 0.0801 && output < 0.0803);
        }

        [TestMethod]
        public void TestDenormaliser_For_DenormaliseAsymptotic()
        {
            var output = Numbery.Denormalise(0.080247933884297518, 100, Numbery.NormalisationMethod.Asymptotic);

            Assert.IsTrue(output > 35.9 && output < 36.1);
        }

        [TestMethod]
        public void TestNormaliser_For_NormaliseDenormaliseAsymptotic()
        {
            var input = 10.0;
            var normalised = Numbery.Normalise(input, 5, 55, 25, 775, Numbery.NormalisationMethod.Asymptotic);
            var output = Numbery.Denormalise(normalised, 25, 775, 5, 55, Numbery.NormalisationMethod.Asymptotic);

            Assert.IsTrue(output < input + 0.000001);
            Assert.IsTrue(output > input - 0.000001);
        }
    }
}
