using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FoldTests
    {
        [TestMethod]
        public void All()
        {
            var op1 = Operation.Success("Hello");
            var op2 = Operation.Success("World");

            var all = new[] { op1, op2 }.Fold((a, s) => a + " " + s);

            Assert.AreEqual("Hello World", all.Result);
        }

        [TestMethod]
        public void AllWithFailure()
        {
            var op1 = Operation.Success("Hello");
            var op2 = Operation.Create(() =>
            {
                var condition = true;      //Prevent "Unreachable Code Detected"
                if (condition) throw new Exception("Something Bad Happened");
                return "";
            });

            var all = new[] { op1, op2 }.Fold((a, s) => a + " " + s);


            Assert.IsNull(all.Result);
            Assert.IsFalse(all.Succeeded);
            Assert.AreEqual("Something Bad Happened", all.Message);
        }


        [TestMethod]
        public void GenericAll()
        {
            var op1 = Operation.Success("Hello");
            var op2 = Operation.Success("World");

            var all = new[] { op1, op2 }.Fold((ag, e) => ag + " " + e);

            Assert.IsTrue(all.Succeeded);
        }

        [TestMethod]
        public void GenericAllWithFailure()
        {
            var op1 = Operation.Success("Hello");
            var op2 = Operation.Create(() =>
            {
                var condition = true;      //Prevent "Unreachable Code Detected"
                if (condition) throw new Exception("Something Bad Happened");
                return "";
            });

            var all = new[] { op1, op2 }.Fold((ag, e) => ag + " " + e);

            Assert.IsFalse(all.Succeeded);
            Assert.AreEqual("Something Bad Happened", all.Message);
        }
    }
}
