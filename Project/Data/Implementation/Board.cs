using Data.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Implementation
{
    internal class Board : API.DataApi
    {
        public override double BoardWidth => 800;
        public override double BoardHeight => 600;

        private readonly List<IBall> _balls = new List<IBall>();

        public override IBall CreateBall(double x, double y, double rad, double speedX, double speedY)
        {
            var ball = new Ball(x, y, rad, speedX, speedY);
            _balls.Add(ball);
            return ball;
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _balls;
        }

        public override void ClearBalls()
        {
            _balls.Clear();
        }
    }
}
