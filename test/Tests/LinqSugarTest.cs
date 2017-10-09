using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class LinqSugarTest
    {
        private Operation _op1;
        private Operation<string> _op2;
        private Operation<DateTime> _op3;
        private Operation<string> _fail;
        public LinqSugarTest()
        {
            _op1 = new Operation
            {
                Message = "Operation 1 Suceeded",
                Succeeded = true
            };
            _op2 = new Operation<string>
            {
                Message = "Operation 2 Suceeded",
                Succeeded = true,
                Result = "Resulting String"
            };
            _op3 = new Operation<DateTime>
            {
                Message = "Operation 3 Succeeded",
                Succeeded = true,
                Result = DateTime.Now
            };
            _fail = new Operation<string>
            {
                Message = "Test Failed",
                Result = "Failed",
                Succeeded = false
            };
        }
        [TestMethod]
        public void LinqSyntaxTest()
        {
            //Act
            var allops = from o2 in _op2
                         from o3 in _op3
                         let o = o2
                         select o + o3 into g
                         from f in _fail
                         select g + f;

            //Assert
            Assert.IsFalse(allops.Succeeded);
            Assert.AreEqual(allops.Message, _fail.Message);
        }

        [TestMethod]
        public void LinqOperationFirstTest()
        {
            var allOps = from o1 in _op1
                         from o2 in _op2
                         select o2;
            Assert.IsTrue(allOps.Succeeded);
        }

        [TestMethod]
        public void LinqOperationSecondTest()
        {
            var allOps = from o1 in _op2
                         from o2 in _op1
                         select o2;
            Assert.IsTrue(allOps.Succeeded);
        }

        [TestMethod]
        public void LinqWithEnumerableFirst()
        {
            var list = new[] { 1, 2, 3 }.Select(x => x.ToString());

            var query = from x in list
                        from y in _op2
                        select x + " " + y;

            var expected = new[] { "1 Resulting String", "2 Resulting String", "3 Resulting String" };
            var result = query.Select(r => r.Result).ToArray();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void LinqWithOperationFirst()
        {
            var list = new[] { 1, 2, 3 };

            var query = from y in _op2
                        from x in list
                        select x + " " + y;

            var expected = new[] { "1 Resulting String", "2 Resulting String", "3 Resulting String" };
            var result = query.Result.ToArray();
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void LinqAsyncTest()
        {
            var asyncTest = Task.Factory.StartNew(async () =>
            {
                var t1 = Operation.Run(async () => await Task.FromResult(1));

                //Due to Compiler Restrictions, you can only put await on the the first Operation
                var allOps = from o1 in await t1
                             from o2 in _op2
                             select o1.ToString() + o2;

                //Assert
                Assert.IsTrue(allOps.Succeeded);
                Assert.AreEqual(1.ToString() + _op2.Result, allOps.Result);
            });
            asyncTest.Wait();
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
            var ops1 = _op1.Next(() => _fail).Next(op => _op3);
            var ops2 = _op2.Next(op => _fail).Next(op => _op1);
            var ops3 = _op2.Next(op => _op1).Next(() => _fail);

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
