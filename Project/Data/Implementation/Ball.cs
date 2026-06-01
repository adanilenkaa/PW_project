using Data.API;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Data.Implementation
{
    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        private double _speedX;
        private double _speedY;
        private bool _stop = false;
        private Task _task;
        private readonly ReaderWriterLockSlim _speedLock = new ReaderWriterLockSlim();
        private double _boardWidth;
        private double _boardHeight;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Ball(double x, double y, double rad, double speedX, double speedY, double weight)
        {
            _x = x;
            _y = y;
            Rad = rad;
            _speedX = speedX;
            _speedY = speedY;
            Weight = weight;
        }

        public double X { get => _x; private set { if (_x != value) { _x = value; OnPropertyChanged(); } } }
        public double Y { get => _y; private set { if (_y != value) { _y = value; OnPropertyChanged(); } } }
        public double Rad { get; }
        public double Weight { get; }

        public double SpeedX
        {
            get
            {
                _speedLock.EnterReadLock();
                try { return _speedX; }
                finally { _speedLock.ExitReadLock(); }
            }
            set
            {
                _speedLock.EnterWriteLock();
                try { _speedX = value; }
                finally { _speedLock.ExitWriteLock(); }
            }
        }

        public double SpeedY
        {
            get
            {
                _speedLock.EnterReadLock();
                try { return _speedY; }
                finally { _speedLock.ExitReadLock(); }
            }
            set
            {
                _speedLock.EnterWriteLock();
                try { _speedY = value; }
                finally { _speedLock.ExitWriteLock(); }
            }
        }

        public void StartMovement(int interval, double boardWidth, double boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            _stop = false;
            _task = Task.Run(async () =>
            {
                Stopwatch realTimeClock = Stopwatch.StartNew();
                double lastTime = 0;

                while (!_stop)
                {
                    var sw = Stopwatch.StartNew();

                    // Obliczanie fizycznego upływu czasu (Real-time Programming)
                    double currentTime = realTimeClock.Elapsed.TotalSeconds;
                    double deltaTime = currentTime - lastTime;
                    lastTime = currentTime;

                    
                    Move(deltaTime);

                    sw.Stop();

                    int sleepTime = interval - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) await Task.Delay(sleepTime);
                }
            });
        }

        public void StopMovement() => _stop = true;

        
        private void Move(double deltaTime)
        {
          
            double timeFactor = deltaTime * 60;

            double currentSpeedX = SpeedX;
            double currentSpeedY = SpeedY;

            double newX = X + (currentSpeedX * timeFactor);
            double newY = Y + (currentSpeedY * timeFactor);

            
            if (newX - Rad < 0)
            {
                SpeedX = Math.Abs(currentSpeedX);
                newX = Rad;
            }
            else if (newX + Rad > _boardWidth)
            {
                SpeedX = -Math.Abs(currentSpeedX);
                newX = _boardWidth - Rad;
            }

            if (newY - Rad < 0)
            {
                SpeedY = Math.Abs(currentSpeedY);
                newY = Rad;
            }
            else if (newY + Rad > _boardHeight)
            {
                SpeedY = -Math.Abs(currentSpeedY);
                newY = _boardHeight - Rad;
            }

            X = newX;
            Y = newY;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}