using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.API;

namespace PresentationModel
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;

        public BallModel(IBall ball)
        {
            _ball = ball;
            _ball.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IBall.X)) OnPropertyChanged(nameof(Left));
                if (args.PropertyName == nameof(IBall.Y)) OnPropertyChanged(nameof(Top));
            };
        }

        public double Left => _ball.X - _ball.Rad;
        public double Top => _ball.Y - _ball.Rad;

        public double Diameter => _ball.Rad * 2;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}