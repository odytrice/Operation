using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MapTests
    {
        private Operation _op1;
        private Operation<string> _op2;
        private Operation<DateTime> _op3;
        private Operation<string> _fail;
        public MapTests()
        {
            _op1 = Operation.Success();
            _op2 = Operation.Success("Resulting String");
            _op3 = Operation.Success(DateTime.Now);
            _fail = Operation.Fail<string>("Test Failed");
        }
        [TestMethod]
        public void OperationMapSuccess()
        {
            //Arrange
            var methods = new Methods();
            //Act
            Operation.Create(methods.Void).Map(methods.Void);
            Operation.Create(methods.ReturnInt).Map(methods.TakeInt);
            Operation.Create(methods.ReturnInt).Map(methods.TakeAndReturnInt);
            Operation.Create(methods.Void).Map(methods.TakeObject);
            Operation.Success().Map(() => methods.ReturnInt());
        }

        [TestMethod]
        public void FailedOperationMap()
        {
            var methods = new Methods();

            var op = Operation.Fail<int>("Failed").Map(methods.TakeInt);
            var op2 = Operation.Fail<int>("Failed").Map(methods.TakeAndReturnInt);

            var op3 = Operation.Fail("Failed").Map(methods.Void);
            var op4 = Operation.Fail("Failed").Map(methods.ReturnInt);


            Assert.IsFalse(op.Succeeded);
            Assert.IsFalse(op2.Succeeded);
            Assert.IsFalse(op3.Succeeded);
            Assert.IsFalse(op4.Succeeded);
            Assert.AreEqual("Failed", op.Message);
            Assert.AreEqual("Failed", op2.Message);
            Assert.AreEqual("Failed", op3.Message);
            Assert.AreEqual("Failed", op4.Message);
        }
    }
}
