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
        private bool _stop = false;
        private Task _task;
        private readonly ReaderWriterLockSlim _speedLock = new ReaderWriterLockSlim();
        private readonly object _positionLock = new object();
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
            _stop = false;
            _task = Task.Run(async () =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (!_stop)
                {
                    double deltaTime = sw.Elapsed.TotalSeconds;
                    sw.Restart();

                    Move(deltaTime);

                    int sleepTime = interval - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) await Task.Delay(sleepTime);
                }
            });
        }

        public void StopMovement() => _stop = true;

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

        private void Move(double deltaTime)
        {
            if (deltaTime > 0.05) deltaTime = 0.05;

            double newX, newY;
            lock (_positionLock)
            {
                double vx = SpeedX;
                double vy = SpeedY;

                newX = _x + vx * deltaTime;
                newY = _y + vy * deltaTime;

                if (newX - Rad < 0 || newX + Rad > _boardWidth)
                {
                    SpeedX = -vx;
                    newX = _x + SpeedX * deltaTime;
                }
                if (newY - Rad < 0 || newY + Rad > _boardHeight)
                {
                    SpeedY = -vy;
                    newY = _y + SpeedY * deltaTime;
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