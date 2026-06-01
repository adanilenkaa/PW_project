using Data.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Implementation
{
    internal class Board : API.DataApi
    {
        public override double BoardWidth => 800;
        public override double BoardHeight => 600;

        private readonly List<IBall> _balls = new List<IBall>();
        private BallLogger _logger;
        private bool _isLogging = false;

        public override IBall CreateBall(double x, double y, double rad, double speedX, double speedY, double weight = 1.0)
        {
            var ball = new Ball(x, y, rad, speedX, speedY, weight);
            _balls.Add(ball);
            return ball;
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _balls;
        }

        public override void ClearBalls()
        {
            StopLogging();
            _balls.Clear();
        }

       
        public override void StartLogging()
        {
            if (_isLogging) return; 

            _logger = new BallLogger();
            _isLogging = true;

            
            Task.Run(async () =>
            {
                while (_isLogging)
                {
                   
                    _logger.LogState(_balls);
                    await Task.Delay(100);
                }
            });
        }

        
        public override void StopLogging()
        {
            _isLogging = false;
            _logger?.Stop();
            _logger = null;
        }
    }
}