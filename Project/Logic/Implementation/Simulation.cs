using Data.API;
using Logic.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Implementation
{
    internal class Simulation : Api.LogicApi
    {
        private Timer _timer;
        private readonly DataApi _dataApi;
        private readonly Random _random = new Random();

        public Simulation(DataApi dataApi)
        {
            _dataApi = dataApi;
        }

        public override void GenerateBalls(int count)
        {

            _dataApi.ClearBalls();
            for (int i = 0; i < count; i++)
            {
                double rad = 15;
                double x = _random.NextDouble() * (_dataApi.BoardWidth- rad*2)+rad;
                double y = _random.NextDouble() * (_dataApi.BoardHeight - rad * 2)+rad;

                double speedX =  (_random.NextDouble()-0.5) * 6;
                double speedY = (_random.NextDouble()-0.5) * 6;
                _dataApi.CreateBall(x, y, rad, speedX, speedY);
            }
        }
        public override void Start()
        {
            _timer = new Timer(MoveBalls, null, 0, 16);
        }

        public override void Stop()
        {
            _timer?.Dispose();
        }

        public override IEnumerable<IBall> GetBalls()
        {
            return _dataApi.GetBalls();
        }

        public override double GetBoardWidth()
        {
            return _dataApi.BoardWidth;
        }

        public override double GetBoardHeight()
        {
            return _dataApi.BoardHeight;
        }

        private void MoveBalls(object state)
        {
            foreach (var ball in _dataApi.GetBalls().ToList())
            {
                ball.Move(_dataApi.BoardWidth, _dataApi.BoardHeight);
            }
        }

    }
}
