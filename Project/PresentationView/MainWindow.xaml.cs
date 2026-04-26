using System.Windows;
using Presentation.ViewModel;

namespace PresentationView
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // To jest kluczowe połączenie Widoku z Logiką (ViewModel)
            this.DataContext = new MainViewModel();
        }
    }
}