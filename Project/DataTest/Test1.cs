using Data.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTest
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestBallCreation()
        {
            Ball ball = new Ball(5, 10);

            Assert.AreEqual(5, ball.X);
            Assert.AreEqual(10, ball.Y);
        }
    }
}
