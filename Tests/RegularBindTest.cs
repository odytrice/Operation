using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class RegularBindTest
    {
        private Operation _op1;
        private Operation<string> _op2;
        private Operation<DateTime> _op3;
        private Operation<string> _fail;
        public RegularBindTest()
        {
            _op1 = new Operation
            {
                Message = "Operation 1 Suceeded",
                Success = true
            };
            _op2 = new Operation<string>
            {
                Message = "Operation 2 Suceeded",
                Success = true,
                Result = "Resulting String"
            };
            _op3 = new Operation<DateTime>
            {
                Message = "Operation 3 Succeeded",
                Success = true,
                Result = DateTime.Now
            };
            _fail = new Operation<string>
            {
                Message = "Test Failed",
                Result = "Failed",
                Success = true
            };
        }
        [TestMethod]
        public void OperationNext()
        {
            //Act
            var allops = _op1.Next(() => _op2).Next((r) => _op3);

            //Assert
            Assert.IsTrue(allops.Success);
            Assert.AreEqual(allops.Message, _op3.Message);
        }
        [TestMethod]
        public void OperationNextRegularMethods()
        {
            //Arrange
            var methods = new Methods();
            //Act
            var allOps = Operation.Create(methods.Print).Next(() => methods.Print());
        }
        [TestMethod]
        public void OperationNextFailed()
        {
            //Act
            var ops1 = _op1.Next(() => _fail).Next((op) => _op3);
            var ops2 = _op2.Next((op) => _fail).Next(() => _op1);
            var ops3 = _op2.Next((op) => _op1).Next(() => _fail);

            //Assert
            Assert.IsFalse(ops1.Success);
            Assert.AreEqual(ops1.Message, _fail.Message);

            Assert.IsFalse(ops2.Success);
            Assert.AreEqual(ops2.Message, _fail.Message);

            Assert.IsFalse(ops3.Success);
            Assert.AreEqual(ops3.Message, _fail.Message);
        }
    }
}
