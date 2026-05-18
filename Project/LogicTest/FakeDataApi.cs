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

        public override IBall CreateBall(double x, double y, double rad, double speedX, double speedY, double weight = 1.0)
        {
            var ball = new FakeBall(x, y, rad, speedX, speedY, weight);
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
        public double Weight { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FakeBall(double x, double y, double rad, double sx, double sy, double weight = 1.0)
        {
            X = x;
            Y = y;
            Rad = rad;
            SpeedX = sx;
            SpeedY = sy;
            Weight = weight;
        }

        public void StartMovement(int interval, double boardWidth, double boardHeight) { }
        public void StopMovement() { }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}