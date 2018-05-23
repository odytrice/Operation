using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AsyncTests
    {
        [TestMethod]
        public async Task AwaitSuccessfulOperation()
        {
            var operation = Operation.Success(10);

            var result = await operation;

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public async Task AwaitFailedOperation()
        {
            var operation = Operation.Fail<int>("An Error Occured");

            bool failed = false;
            try
            {
                var result = await operation;
            }
            catch
            {
                failed = true;
            }

            Assert.IsTrue(failed, "Operation Not Throwing Exception");
        }

        [TestMethod]
        public void TestErrorCapturedInTask()
        { 
            var result = AsyncOpMethod();

            Assert.IsTrue(result.IsFaulted);

            async Task<int> AsyncOpMethod()
            {
                await Operation.Fail<int>("An Error Occured");
                return 1;
            }
        }

        [TestMethod]
        public async Task AwaitNonGenericOperation()
        {
            var operation = Operation.Success();
            await operation;
        }
    }
}
