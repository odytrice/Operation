using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Operation = System.Operation;

namespace Tests
{
    /// <summary>
    /// Test Suite for Operation Unit Functions
    /// </summary>
    [TestClass]
    public class UnitTest
    {
        private Methods _methods;
        public UnitTest()
        {
            _methods = new Methods();
        }

        [TestMethod]
        public void OperationCreationSuccess()
        {
            var operation = Operation.Create(_methods.Print);

            Assert.IsTrue(operation.Succeeded);
        }

        [TestMethod]
        public void OperationCreationFailure()
        {
            var operation = Operation.Create(() =>
            {
                throw new Exception("The Error");
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }

        [TestMethod]
        public void OperationResult()
        {
            var operation = Operation.Create<int>(_methods.ReturnInt);

            Assert.AreEqual(operation.Result, 1);
            Assert.IsTrue(operation.Succeeded);
        }

        [TestMethod]
        public void OperationResultFailure()
        {
            var cond = true;
            var operation = Operation.Create(() =>
            {
                if (cond) throw new Exception("The Error");
                return 1;
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
            Assert.AreEqual(operation.Result, default(int));
        }
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
        public void AsyncOperationCreationFailure()
        {
            var task = Operation.Run(async () =>
            {
                await Task.Run(() => Console.WriteLine("Hello Operation"));
                throw new Exception("The Error");
            });

            task.Wait();

            Assert.IsFalse(task.Result.Succeeded);
            Assert.AreEqual(task.Result.Message, "The Error");
        }

        [TestMethod]
        public void OperationFail()
        {
            var message = "Evil Error";
            var op1 = Operation.Fail(message);
            var op2 = Operation.Fail<int>(message);

            Assert.AreEqual(message, op1.Message);
            Assert.AreEqual(default(int), op2.Result);
        }

        [TestMethod]
        public void OperationSuccess()
        {
            var op1 = Operation.Success();
            var op2 = Operation.Success(1000);

            Assert.IsTrue(op1.Succeeded);
            Assert.IsTrue(op2.Succeeded);
            Assert.AreEqual(1000, op2.Result);
        }
    }
}
