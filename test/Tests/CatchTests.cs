using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CatchTests
    {
        [TestMethod]
        public void CatchT()
        {
            var message = "";
            var op1 = Operation.Fail<string>("An Error").Catch(o => message = o.Message);
            Assert.AreEqual("An Error", message);
        }

        [TestMethod]
        public void Catch()
        {
            var message = "";
            var op1 = Operation.Fail("An Error").Catch(o => message = o.Message);
            Assert.AreEqual("An Error", message);
        }
    }
}
