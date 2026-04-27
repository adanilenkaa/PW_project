using Data.API;
using System.Collections.Generic;
using System.ComponentModel;

namespace LogicTest
{
    internal class FakeDataApi : DataApi
    {
        public override double BoardWidth => 800;
        public override double BoardHeight => 600;
        private readonly List<IBall> _balls = new List<IBall>();

        public override IBall CreateBall(double x, double y, double rad, double speedX, double speedY)
        {
            var ball = new FakeBall(x, y, rad);
            _balls.Add(ball);
            return ball;
        }

        public override IEnumerable<IBall> GetBalls() => _balls;
        public override void ClearBalls() => _balls.Clear();
    }

    internal class FakeBall : IBall
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Rad { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public FakeBall(double x, double y, double rad) { X = x; Y = y; Rad = rad; }
        public void Move(double bw, double bh) { /* Tutaj pusto, bo testujemy tylko logikę */ }
    }
}
