using Data.API;
using Data.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [TestMethod]
        public void TestThreadSafeSpeedAccess()
        {
            DataApi api = DataApi.Create();
            IBall ball = api.CreateBall(100, 100, 15, 5, 5);

            int readCount = 0;
            int writeCount = 0;

            var readTask = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    var speedX = ball.SpeedX;
                    var speedY = ball.SpeedY;
                    readCount++;
                }
            });

            var writeTask = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    ball.SpeedX = 3 + i;
                    ball.SpeedY = 4 + i;
                    writeCount++;
                }
            });

            Task.WaitAll(readTask, writeTask);

            Assert.AreEqual(100, readCount, "Should complete all reads without exception");
            Assert.AreEqual(100, writeCount, "Should complete all writes without exception");
        }

        [TestMethod]
        public void TestDiagnosticLoggingDoesNotThrow()
        {
            DataApi api = DataApi.Create();
            IBall ball = api.CreateBall(10, 10, 15, 2, 2);

            string tempPath = System.IO.Path.GetTempFileName();
            api.StartDiagnosticLogging(tempPath);

            System.Threading.Thread.Sleep(200);

            api.StopDiagnosticLogging();

             Assert.IsTrue(System.IO.File.Exists(tempPath));
        }
    }
}

