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
    }
}