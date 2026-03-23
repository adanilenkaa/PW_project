using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Point> InitialBallPositions { get; private set; }

        public MainWindow()
        {
            InitializeComponent();


            InitialBallPositions = new List<Point>
            {
                new Point(10, 10),
                new Point(50, 50),
                new Point(100, 100)
            };
        }
    }
}