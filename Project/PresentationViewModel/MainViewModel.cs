using PresentationViewModel;
using PresentationModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PresentationViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ModelApi _model;
        private int _ballCount = 5;

        public ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public MainViewModel()
        {
            _model = ModelApi.CreateApi();

            StartCommand = new RelayCommand(() => {
                _model.Start(BallCount);
                Balls.Clear();
                foreach (var ball in _model.GetBalls())
                {
                    Balls.Add(ball);
                }
            });

            StopCommand = new RelayCommand(() => {
                _model.Stop();
            });
        }

        public int BallCount
        {
            get => _ballCount;
            set { _ballCount = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}