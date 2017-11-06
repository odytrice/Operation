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
    public class CreateTests
    {
        private Methods _methods;
        public CreateTests()
        {
            _methods = new Methods();
        }

        [TestMethod]
        public void CreateSuccess()
        {
            var operation = Operation.Create(_methods.Void);
            Assert.IsTrue(operation.Succeeded);
        }

        [TestMethod]
        public void CreateFailure()
        {
            var operation = Operation.Create(() =>
            {
                throw new Exception("The Error");
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual(operation.Message, "The Error");
        }

        [TestMethod]
        public void CreateBindSuccess()
        {
            var operation = Operation.CreateBind(() =>
            {
                return Operation.Success(3);
            });

            Assert.IsTrue(operation.Succeeded);
            Assert.AreEqual(3, operation.Result);
        }

        [TestMethod]
        public void CreateBindFailure()
        {
            var operation = Operation.CreateBind(() => Operation.Fail("An Error Occured"));

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual("An Error Occured", operation.Message);
        }

        [TestMethod]
        public void CreateBindCatchesExceptions()
        {
            var operation = Operation.CreateBind(() =>
            {
                var x = true;
                if (x) throw new Exception("Some Error");
                return Operation.Success(2);
            });

            Assert.IsFalse(operation.Succeeded);
            Assert.AreEqual("Some Error", operation.Message);
        }

        [TestMethod]
        public void CreateWithResult()
        {
            var operation = Operation.Create(_methods.ReturnInt);

            Assert.AreEqual(operation.Result, 1);
            Assert.IsTrue(operation.Succeeded);
        }

        [TestMethod]
        public void CreateWithResultFailure()
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
