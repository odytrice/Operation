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
        public void AsyncOperationCreationSuccess()
        {
            var task = Operation.Run(async () =>
            {
                await Task.Run(() => Console.WriteLine("Hello Operation"));
            });

            task.Wait();

            Assert.IsTrue(task.Result.Succeeded);
        }

        [TestMethod]
        public async Task AsyncOperationCreationFailure()
        {
            var task = Operation.Run(() =>
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Hello Operation");
                    throw new Exception("The Error");
                });
            });

            var result = await task;

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(result.Message, "The Error");
        }

        [TestMethod]
        public async Task AsyncOperationTCreationFailure()
        {
            var task = Operation.Run(() =>
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Hello Operation");
                    var x = true;
                    if(x) throw new Exception("The Error");
                    return 1;
                });
            });

            var result = await task;

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(result.Message, "The Error");
        }
    }
}
