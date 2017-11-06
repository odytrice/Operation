using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class BindTests
    {
        private Operation _op1;
        private Operation<string> _op2;
        private Operation<DateTime> _op3;
        private Operation<string> _fail;
        public BindTests()
        {
            _op1 = Operation.Success();
            _op2 = Operation.Success("Resulting String");
            _op3 = Operation.Success(DateTime.Now);
            _fail = Operation.Fail<string>("Test Failed");
        }
        [TestMethod]
        public void OperationBind()
        {
            //Act
            var allops = _op1.Bind(() => _op2).Bind(r => _op3);

            //Assert
            Assert.IsTrue(allops.Succeeded);
            Assert.AreEqual(allops.Message, _op3.Message);
        }

        [TestMethod]
        public void OperationBindFailed()
        {
            //Act
            var ops1 = _op1.Bind(() => _fail).Bind((op) => _op3);
            var ops2 = _op2.Bind((op) => _fail).Bind(op => _op1);
            var ops3 = _op2.Bind((op) => _op1).Bind(() => _fail);

            //Assert
            Assert.IsFalse(ops1.Succeeded);
            Assert.AreEqual(ops1.Message, _fail.Message);

            Assert.IsFalse(ops2.Succeeded);
            Assert.AreEqual(ops2.Message, _fail.Message);

            Assert.IsFalse(ops3.Succeeded);
            Assert.AreEqual(ops3.Message, _fail.Message);
        }
    }
}
