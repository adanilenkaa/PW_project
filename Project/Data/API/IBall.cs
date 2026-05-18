using System.ComponentModel;

namespace Data.API
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double Rad { get; }
        double Weight { get; }
        double SpeedX { get; set; }
        double SpeedY { get; set; }

        void StartMovement(int interval, double boardWidth, double boardHeight);
        void StopMovement();
    }
}