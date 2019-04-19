using System;
using System.Collections.Generic;
using GeneticAlgorithm;
using GeneticAlgorithm.TraitTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneticAlgorithmTests
{
    [TestClass]
    public class OrganismTests
    {
        [TestMethod]
        public void DNAEncodeDecodeShouldMatchInputs()
        {
            //Arrange
            var valueBool = true;
            var valueInteger = 5;
            var valueIntegerList = new List<int>() {2, 4, 7};
            var valueDouble = 0.66;
            var valueString = "Banana!";

            var organismIn = new Organism();
            var genes = new List<Gene>
            {
                new GeneBool() {Decoded = valueBool, Reference = "valueBool"},
                new GeneInt32() {Decoded = valueInteger, Reference = "valueInteger"},
                new GeneListInt32() {Decoded = valueIntegerList, Reference = "valueIntegerList"},
                new GeneDouble() {Decoded = valueDouble, Reference = "valueDouble"},
                //new GenePlainText() {Decoded = valueString, Reference = "valueString"},
            };
            
            var organismOut = new Organism();

            //Act
            organismIn.Genes = genes;
            organismIn.Encode();
            var temp = organismIn.Genes;
            foreach (var gene in temp)
            {
                gene.Decoded = 0;
                organismOut.Genes.Add(gene);
            }
            organismIn.Decode();
            var result = organismOut.Genes;

            //Assert
            Assert.AreEqual(valueBool, result[0].Decoded);
            Assert.AreEqual(valueInteger, result[1].Decoded);
            //Assert.AreEqual(valueIntegerList, result[2].Decoded);
            Assert.IsTrue(result[2].Decoded > valueDouble - 0.000001);
            Assert.IsTrue(result[2].Decoded < valueDouble + 0.000001);
            //Assert.AreEqual(valueString, result[4].Decoded);
        }
    }
}
