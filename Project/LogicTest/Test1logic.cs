using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic.Api;
using System.Linq;

namespace LogicTest
{
    [TestClass]
    public class LogicUnitTest
    {
        [TestMethod]
        public void TestBallGeneration()
        {
            LogicApi api = LogicApi.CreateApi(new FakeDataApi());

            api.GenerateBalls(5);
            var balls = api.GetBalls();

            Assert.AreEqual(5, balls.Count());
        }

        [TestMethod]
        public void TestElasticCollision()
        {
            FakeDataApi dataApi = new FakeDataApi();

            var ball1 = dataApi.CreateBall(100, 100, 15, 5, 0, 1.0);
            var ball2 = dataApi.CreateBall(130, 100, 15, -5, 0, 1.0);

            double initialSpeed1 = ball1.SpeedX;
            double initialSpeed2 = ball2.SpeedX;

            double m1 = ball1.Weight;
            double m2 = ball2.Weight;

            double v1x_new = ((m1 - m2) * initialSpeed1 + 2 * m2 * initialSpeed2) / (m1 + m2);
            double v2x_new = ((m2 - m1) * initialSpeed2 + 2 * m1 * initialSpeed1) / (m1 + m2);

            Assert.AreEqual(-5, v1x_new, "After collision, ball1 should have ball2's velocity");
            Assert.AreEqual(5, v2x_new, "After collision, ball2 should have ball1's velocity");
        }
    }
}