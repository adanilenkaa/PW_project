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
            // 1. Arrange: Tworzymy logikę, wstrzykując naszego Fake'a (Dependency Injection)
            LogicApi api = LogicApi.CreateApi(new FakeDataApi());

            // 2. Act: Chcemy wygenerować 5 kul
            api.GenerateBalls(5);
            var balls = api.GetBalls();

            // 3. Assert: Sprawdzamy, czy faktycznie jest ich 5
            Assert.AreEqual(5, balls.Count());
        }
    }
}