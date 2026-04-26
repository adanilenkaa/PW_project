using System.Collections.ObjectModel;
using System.Windows.Input;
using Presentation.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Presentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ModelApi _model;
        private int _ballCount = 5; 
        private readonly DispatcherTimer _timer;

        public ObservableCollection<object> Balls { get; } = new ObservableCollection<object>();

        public MainViewModel()
        {
            _model = ModelApi.CreateApi();
            _timer = new DispatcherTimer();
            _timer.Interval = System.TimeSpan.FromMilliseconds(16);
            _timer.Tick += (s, e) => {
                // To wymusza przerysowanie pozycji kul na Canvasie
                OnPropertyChanged(nameof(Balls));
            };

            StartCommand = new RelayCommand(() => {
                _model.Start(BallCount);

                // Pobieramy kule z logiki i dodajemy do kolekcji widocznej w GUI
                Balls.Clear();
                foreach (var ball in _model.GetBalls())
                {
                    Balls.Add(ball);
                }

                _timer.Start();
            });

            StopCommand = new RelayCommand(() => {
                _model.Stop();
                _timer.Stop();
            });
        }

        public int BallCount
        {
            get => _ballCount;
            set { _ballCount = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}