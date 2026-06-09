using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.API;

namespace PresentationModel
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;
        private double _canvasWidth = 800;
        private double _canvasHeight = 600;

        public string Color
        {
            get => _ball.Color;
        }

        public BallModel(IBall ball, double canvasWidth = 800, double canvasHeight = 600)
        {
            _ball = ball;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;

            _ball.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IBall.X)) OnPropertyChanged(nameof(Left));
                if (args.PropertyName == nameof(IBall.Y)) OnPropertyChanged(nameof(Top));
                if (args.PropertyName == nameof(IBall.Color)) OnPropertyChanged(nameof(Color));
            
            };
        }

        public double Left => (_ball.X - _ball.Rad);
        public double Top => (_ball.Y - _ball.Rad);

        public double Diameter => _ball.Rad * 2;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}