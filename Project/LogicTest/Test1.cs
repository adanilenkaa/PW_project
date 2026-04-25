using Logic.Implementation;

namespace BusinessLogicTest
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestCreateBalls()
        {

            LogicApi api = new LogicApi();

            var balls = api.CreateBalls();


            Assert.HasCount(2, balls);
            Assert.AreEqual(10, balls[0].X);
            Assert.AreEqual(10, balls[0].Y);
        }
    }
}
