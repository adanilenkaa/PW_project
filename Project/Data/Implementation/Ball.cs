using Data.API;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Data.Implementation
{
    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        private double _speedX;
        private double _speedY;
        private string _color;

        private Timer _timer;
        private int _updateInterval;
        private int _timeSinceColorChange = 0; 

        private readonly ReaderWriterLockSlim _speedLock = new ReaderWriterLockSlim();
        private readonly object _positionLock = new object();
        private double _boardWidth;
        private double _boardHeight;

        private static readonly string[] Colors = { "Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Cyan", "Magenta" };
        private readonly Random _random = new Random();

        public event PropertyChangedEventHandler? PropertyChanged;

        public Ball(double x, double y, double rad, double speedX, double speedY, double weight)
        {
            _x = x;
            _y = y;
            Rad = rad;
            _speedX = speedX;
            _speedY = speedY;
            Weight = weight;
            _color = Colors[_random.Next(Colors.Length)];
        }

        public double X { get { lock (_positionLock) return _x; } }
        public double Y { get { lock (_positionLock) return _y; } }
        public double Rad { get; }
        public double Weight { get; }

        public string Color
        {
            get => _color;
            private set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

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
            _updateInterval = interval;

            _timer = new Timer(TimerCallback, null, 0, interval);
        }

        public void StopMovement()
        {
            _timer?.Dispose();
        }

        private void TimerCallback(object state)
        {
            Move(); 

            _timeSinceColorChange += _updateInterval;

            if (_timeSinceColorChange >= 2000)
            {
                Color = Colors[_random.Next(Colors.Length)];
                _timeSinceColorChange = 0;
            }
        }

        public void SetPosition(double x, double y)
        {
            lock (_positionLock) { _x = x; _y = y; }
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public void SetSpeed(double vx, double vy)
        {
            _speedLock.EnterWriteLock();
            try { _speedX = vx; _speedY = vy; }
            finally { _speedLock.ExitWriteLock(); }
        }

        private void Move()
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
                newX = _x + currentVx;
                newY = _y + currentVy;

                if (newX - Rad < 0 || newX + Rad > _boardWidth)
                {
                    SetSpeed(-currentVx, currentVy);
                    newX = _x - currentVx;
                }
                if (newY - Rad < 0 || newY + Rad > _boardHeight)
                {
                    SetSpeed(currentVx, -currentVy);
                    newY = _y - currentVy;
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