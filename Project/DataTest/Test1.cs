using Data.API;
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
            DataApi api = DataApi.Create();

            api.CreateBall(5, 10, 15, 2, 2);

            IBall ball = api.GetBalls().First();

            Assert.AreEqual(5, ball.X);
            Assert.AreEqual(10, ball.Y);
        }

        [TestMethod]
        public void TestClearBalls()
        {
            DataApi api = DataApi.Create();
            api.CreateBall(5, 10, 15, 2, 2);

            api.ClearBalls();

            Assert.AreEqual(0, api.GetBalls().Count());
        }
    }
}

