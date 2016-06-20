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
    }
}
