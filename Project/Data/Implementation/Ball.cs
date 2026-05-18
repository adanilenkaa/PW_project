using Data.API;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public double X { get => _x; private set { _x = value; OnPropertyChanged(); } }
        public double Y { get => _y; private set { _y = value; OnPropertyChanged(); } }
        public double Rad { get; }
        public double Weight { get; }
        public double SpeedX { get => _speedX; set => _speedX = value; }
        public double SpeedY { get => _speedY; set => _speedY = value; }

        // Każda kula ma swoją własną pętlę w osobnym wątku
        public void CreateTask(int interval)
        {
            _stop = false;
            _task = Task.Run(async () =>
            {
                while (!_stop)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    Move();
                    sw.Stop();

                    // Czekamy tyle, ile zostało do pełnego interwału
                    int sleepTime = interval - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) await Task.Delay(sleepTime);
                }
            });
        }

        public void StopTask() => _stop = true;

        private void Move()
        {
            X += SpeedX;
            Y += SpeedY;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}