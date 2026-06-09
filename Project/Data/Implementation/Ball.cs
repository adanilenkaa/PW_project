using Data.API;
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

        private readonly ReaderWriterLockSlim _speedLock = new ReaderWriterLockSlim();
        private readonly object _positionLock = new object();
        private double _boardWidth;
        private double _boardHeight;

        private Timer _timer;
        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

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

        public double X { get { lock (_positionLock) return _x; } }
        public double Y { get { lock (_positionLock) return _y; } }
        public double Rad { get; }
        public double Weight { get; }

        public double SpeedX
        {
            get { _speedLock.EnterReadLock(); try { return _speedX; } finally { _speedLock.ExitReadLock(); } }
            set { _speedLock.EnterWriteLock(); try { _speedX = value; } finally { _speedLock.ExitWriteLock(); } }
        }

        public double SpeedY
        {
            get { _speedLock.EnterReadLock(); try { return _speedY; } finally { _speedLock.ExitReadLock(); } }
            set { _speedLock.EnterWriteLock(); try { _speedY = value; } finally { _speedLock.ExitWriteLock(); } }
        }

        public void StartMovement(int interval, double boardWidth, double boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;

            _stopwatch.Start();
            _timer = new Timer(TimerCaller, null, 0, interval);
        }
        public void StopMovement()
        {
            _timer?.Dispose();
            _stopwatch.Stop();
        }


        private void TimerCaller(object state)
        {
            double deltaTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();

            if (deltaTime > 0.05) deltaTime = 0.05;

            Move(deltaTime);
        }

        public void SetPosition(double x, double y)
        {
            lock (_positionLock)
            {
                _x = x;
                _y = y;
            }
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public void SetSpeed(double vx, double vy)
        {
            _speedLock.EnterWriteLock();
            try
            {
                _speedX = vx;
                _speedY = vy;
            }
            finally { _speedLock.ExitWriteLock(); }
        }

        private void Move(double deltaTime)
        {
            double currentVx, currentVy;

            _speedLock.EnterReadLock();
            try
            {
                currentVx = _speedX;
                currentVy = _speedY;
            }
            finally { _speedLock.ExitReadLock(); }

            double newX, newY;

            lock (_positionLock)
            {
                newX = _x + currentVx * deltaTime;
                newY = _y + currentVy * deltaTime;

                if (newX - Rad < 0 || newX + Rad > _boardWidth)
                {
                    SetSpeed(-currentVx, currentVy);
                    newX = _x - currentVx * deltaTime;
                }
                if (newY - Rad < 0 || newY + Rad > _boardHeight)
                {
                    SetSpeed(currentVx, -currentVy);
                    newY = _y - currentVy * deltaTime;
                }

                _x = newX;
                _y = newY;
            }

            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}