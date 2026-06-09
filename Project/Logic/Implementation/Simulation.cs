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

            var tempBallsList = new List<(double X, double Y, double Rad)>();

            for (int i = 0; i < count; i++)
            {
                double rad = 15;
                double x = 0, y = 0;
                bool hasOverlap = true;
                int attempts = 0;

                while (hasOverlap && attempts < 100)
                {
                    x = random.NextDouble() * (_dataApi.BoardWidth - rad * 2) + rad;
                    y = random.NextDouble() * (_dataApi.BoardHeight - rad * 2) + rad;
                    hasOverlap = false;


                    foreach (var existing in tempBallsList)
                    {
                        double dx = x - existing.X;
                        double dy = y - existing.Y;
                        if (Math.Sqrt(dx * dx + dy * dy) <= rad + existing.Rad + 2)
                        {
                            hasOverlap = true;
                            break;
                        }
                    }
                    attempts++;
                }

                double sx = (random.NextDouble() - 0.5) * 4;
                double sy = (random.NextDouble() - 0.5) * 4;

                _dataApi.CreateBall(x, y, rad, sx, sy);
                tempBallsList.Add((x, y, rad));
            }
        }

        public override void Start()
        {
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "billiard_balls.log");
            _dataApi.StartDiagnosticLogging(logPath);

            foreach (var ball in _dataApi.GetBalls())
            {
                _activeBalls.Add(ball);
                ball.PropertyChanged += OnBallPropertyChanged;
                ball.StartMovement(16, _dataApi.BoardWidth, _dataApi.BoardHeight);
            }
        }

        public override void Stop()
        {
            _dataApi.StopDiagnosticLogging();
            foreach (var ball in _dataApi.GetBalls())
            {
                ball.StopMovement();
                ball.PropertyChanged -= OnBallPropertyChanged;
            }
            _activeBalls.Clear();
        }

        private bool _isResolvingCollision = false;

        private void OnBallPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IBall ball = (IBall)sender;

            if (e.PropertyName == nameof(IBall.Y))
            {
                lock (_collisionLock)
                {
                    if (_isResolvingCollision) return; 

                    _isResolvingCollision = true;
                    try
                    {
                        CheckBallCollision(ball);
                    }
                    finally
                    {
                        _isResolvingCollision = false;
                    }
                }
            }
        }

        private void CheckBallCollision(IBall ball)
        {
            foreach (var other in _dataApi.GetBalls())
            {
                if (ball == other) continue;

                double dx = ball.X - other.X;
                double dy = ball.Y - other.Y;
                double distanceSq = dx * dx + dy * dy;
                double distance = Math.Sqrt(distanceSq);
                double minDist = ball.Rad + other.Rad;

                if (distance <= minDist)
                {
                    if (distance == 0) distance = 0.0001;

                    double overlap = minDist - distance;
                    double nx = dx / distance;
                    double ny = dy / distance;

                    double totalMass = ball.Weight + other.Weight;
                    double moveRatio1 = other.Weight / totalMass;
                    double moveRatio2 = ball.Weight / totalMass;

                    ball.SetPosition(ball.X + nx * overlap * moveRatio1, ball.Y + ny * overlap * moveRatio1);
                    other.SetPosition(other.X - nx * overlap * moveRatio2, other.Y - ny * overlap * moveRatio2);

                    double dvx = ball.SpeedX - other.SpeedX;
                    double dvy = ball.SpeedY - other.SpeedY;
                    double dotProduct = dvx * dx + dvy * dy;

                    if (dotProduct < 0)
                    {
                        double collisionScale1 = (2 * other.Weight / totalMass) * (dotProduct / distanceSq);
                        double collisionScale2 = (2 * ball.Weight / totalMass) * (dotProduct / distanceSq);

                        ball.SetSpeed(ball.SpeedX - collisionScale1 * dx, ball.SpeedY - collisionScale1 * dy);
                        other.SetSpeed(other.SpeedX - collisionScale2 * (-dx), other.SpeedY - collisionScale2 * (-dy));
                    }
                }
            }
        }

        public override IEnumerable<IBall> GetBalls() => _dataApi.GetBalls();
        public override double GetBoardWidth() => _dataApi.BoardWidth;
        public override double GetBoardHeight() => _dataApi.BoardHeight;
    }
}