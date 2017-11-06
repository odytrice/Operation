using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnwrapTests
    {
        [TestMethod]
        public void TestOperationThrow()
        {

            var operation = Operation.Create(() =>
            {
                Operation.Fail("The Error").Unwrap();
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }

        [TestMethod]
        public void TestOperationResultUnwrap()
        {
            var operation = Operation.Create(() =>
            {
                var failedOp = Operation.Fail<string>("The Error");
                return failedOp.Unwrap();
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }
    }
}
