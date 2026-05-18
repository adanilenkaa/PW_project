using Data.API;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
                while (!_stop)
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    Move();
                    sw.Stop();

                    int sleepTime = interval - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) await Task.Delay(sleepTime);
                }
            });
        }

        public void StopMovement() => _stop = true;

        private void Move()
        {
            double newX = X + SpeedX;
            double newY = Y + SpeedY;
            if (newX - Rad < 0 || newX + Rad > _boardWidth)
            {
                SpeedX = -SpeedX;
                newX = X + SpeedX;
            }
            if (newY - Rad < 0 || newY + Rad > _boardHeight)
            {
                SpeedY = -SpeedY;
                newY = Y + SpeedY;
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