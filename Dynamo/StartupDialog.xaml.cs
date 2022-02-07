using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dynamo
{
    /// <summary>
    /// Interaction logic for StartupDialog.xaml
    /// </summary>
    public partial class StartupDialog : Window
    {
        public Action NewProjectCallback;
        public Action OpenProjectCallback;

        public StartupDialog()
        {
            InitializeComponent();
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            NewProjectCallback?.Invoke();
        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {
            OpenProjectCallback?.Invoke();
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
