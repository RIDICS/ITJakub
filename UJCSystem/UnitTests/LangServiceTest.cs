using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesLayer;

namespace UnitTests
{
    [TestClass]
    public class LangServiceTest
    {
        static string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras vitae nunc at lacus tempor pretium. Curabitur velit tellus, accumsan et facilisis ullamcorper, vehicula a nunc. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin nunc magna, consequat eu facilisis vitae, hendrerit in magna. Aenean sit amet urna odio. Vestibulum ac leo elit. Cras bibendum nisi vel ligula pretium scelerisque. Cras accumsan libero justo. Pellentesque nec leo id sapien interdum gravida at eget enim. Integer tempor ornare enim in vestibulum. Donec eu elit lectus. Nulla nulla est, viverra id ultrices vel, tincidunt vitae ligula. Suspendisse feugiat sollicitudin consequat.";
        static string taggedText = "<p>Lorem ipsum dolor <od>sit amet, consectetur</do> adipiscing elit. </p>Cras vitae nunc at lacus tempor pretium. Curabitur velit tellus, <hint>accumsan et facilisis ullamcorper, vehicula a nunc. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin nunc magna, consequat eu </hint>facilisis vitae, hendrerit in magna. Aenean sit amet urna odio. Vestibulum ac leo elit. Cras bibendum nisi vel ligula pretium scelerisque. Cras accumsan libero justo. Pellentesque nec leo id sapien interdum gravida at eget enim. Integer tempor ornare enim in vestibulum. Donec eu elit lectus. Nulla nulla est, viverra id ultrices vel, tincidunt vitae ligula. Suspendisse feugiat sollicitudin consequat.";

        static LangService lservice = new LangService();

        [TestMethod]
        public void TestRemoveTags()
        {
            string removed = lservice.RemoveTags(taggedText);
            Assert.AreEqual(removed, text);
        }


        [TestMethod]
        public void TestGetLemma()
        {

        }

        [TestMethod]
        public void TestGetStemma()
        {

        }

        [TestMethod]
        public void TestGetSenetces()
        {

        }

        [TestMethod]
        public void TestWordOrder()
        {
            string[] goodWords = {"consectetur", "Cras", "vitae","pretium", "tellus"};
            string[] badWords = { "consectetur", "Cras", "vitae", "tellus", "elit" };
            Assert.IsTrue(lservice.WordOrder(text,goodWords));
            Assert.IsFalse(lservice.WordOrder(text,badWords));
        }

        [TestMethod]
        public void TestDivideToSubsentences()
        {

        }

        [TestMethod]
        public void TestContainsWords()
        {
            string[] goodWords = {"consectetur", "Cras", "vitae","pretium", "tellus"};
            string[] badWords = { "consectetur", "Cras", "vitae", "pretium", "tellus", "TohleTuNeni" };
            Assert.IsTrue(lservice.ContainsWords(text, goodWords));
            Assert.IsFalse(lservice.ContainsWords(text, badWords));
        }
    }
}
