using System.Windows;
using WindowDocker.Model;

namespace Dynamo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Span _testSpan { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
