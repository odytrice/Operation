using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void TestOperationThrow()
        {
            var subOperation = Operation.Create(() =>
            {
                throw new Exception("The Error");
            });

            var operation = Operation.Create(() =>
            {
                subOperation.Throw();
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }

        [TestMethod]
        public void TestOperationResultUnwrap()
        {
            var subOperation = Operation.Create(() =>
            {
                var condition = true;
                if (condition) throw new Exception("The Error");
                return 1;
            });

            var operation = Operation.Create(() =>
            {
                var result = subOperation.Unwrap();
                return result;
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }


        [TestMethod]
        public void All()
        {
            var op1 = Operation.Create(() => "Hello");
            var op2 = Operation.Create(() => "World");

            var all = new[] { op1, op2 }.Fold((a, s) => a + " " + s);

            Assert.AreEqual("Hello World", all.Result);
        }

        [TestMethod]
        public void AllWithFailure()
        {
            var op1 = Operation.Create(() => "Hello");
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
            var op1 = Operation.Create(() => "Hello");
            var op2 = Operation.Create(() => "World");

            var all = new[] { op1, op2 }.Fold((ag, e) => ag + " " + e);

            Assert.IsTrue(all.Succeeded);
        }

        [TestMethod]
        public void GenericAllWithFailure()
        {
            var op1 = Operation.Create(() => "Hello");
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

        [TestMethod]
        public void CatchT()
        {
            var message = "";
            var op1 = Operation.Fail("An Error").Catch(o => message = o.Message);
            Assert.AreEqual("An Error", message);
        }
    }
}
