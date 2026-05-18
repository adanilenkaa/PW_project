using Data.API;
using Logic.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Logic.Implementation
{
    internal class Simulation : LogicApi
    {
        private readonly DataApi _dataApi;
        private readonly object _collisionLock = new object();
        private readonly HashSet<IBall> _activeBalls = new HashSet<IBall>();

        public Simulation(DataApi dataApi)
        {
            _dataApi = dataApi;
        }

        public override void GenerateBalls(int count)
        {
            _dataApi.ClearBalls();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                double rad = 15;
                double x = random.NextDouble() * (_dataApi.BoardWidth - rad * 2) + rad;
                double y = random.NextDouble() * (_dataApi.BoardHeight - rad * 2) + rad;
                double sx = (random.NextDouble() - 0.5) * 4;
                double sy = (random.NextDouble() - 0.5) * 4;

                _dataApi.CreateBall(x, y, rad, sx, sy);
            }
        }

        public override void Start()
        {
            foreach (var ball in _dataApi.GetBalls())
            {
                _activeBalls.Add(ball);
                ball.PropertyChanged += OnBallPropertyChanged;
                ball.StartMovement(16, _dataApi.BoardWidth, _dataApi.BoardHeight);
            }
        }

        public override void Stop()
        {
            foreach (var ball in _dataApi.GetBalls())
            {
                ball.StopMovement();
                ball.PropertyChanged -= OnBallPropertyChanged;
            }
            _activeBalls.Clear();
        }

        private void OnBallPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IBall ball = (IBall)sender;
            if (e.PropertyName == nameof(IBall.X) || e.PropertyName == nameof(IBall.Y))
            {
                lock (_collisionLock)
                {
                    CheckBallCollision(ball);
                }
            }
        }

        private void CheckBallCollision(IBall ball)
        {
            foreach (var other in _dataApi.GetBalls())
            {
                if (ball == other) continue;

                double dx = other.X - ball.X;
                double dy = other.Y - ball.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= ball.Rad + other.Rad)
                {
                    double dvx = other.SpeedX - ball.SpeedX;
                    double dvy = other.SpeedY - ball.SpeedY;

                    double dotProduct = dx * dvx + dy * dvy;

               
                    if (dotProduct < 0)
                    {
                        double m1 = ball.Weight;
                        double m2 = other.Weight;

                        double v1x = ball.SpeedX;
                        double v1y = ball.SpeedY;
                        double v2x = other.SpeedX;
                        double v2y = other.SpeedY;

                        ball.SpeedX = ((m1 - m2) * v1x + 2 * m2 * v2x) / (m1 + m2);
                        ball.SpeedY = ((m1 - m2) * v1y + 2 * m2 * v2y) / (m1 + m2);

                        other.SpeedX = ((m2 - m1) * v2x + 2 * m1 * v1x) / (m1 + m2);
                        other.SpeedY = ((m2 - m1) * v2y + 2 * m1 * v1y) / (m1 + m2);
                    }
                }
            }
        }

        public override IEnumerable<IBall> GetBalls() => _dataApi.GetBalls();
        public override double GetBoardWidth() => _dataApi.BoardWidth;
        public override double GetBoardHeight() => _dataApi.BoardHeight;
    }
}