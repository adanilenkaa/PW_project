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

        // Metody sterujące autonomicznym ruchem kuli
        void CreateTask(int interval);
        void StopTask();
    }
}