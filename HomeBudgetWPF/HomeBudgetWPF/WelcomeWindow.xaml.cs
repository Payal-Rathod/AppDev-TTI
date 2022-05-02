using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        MainWindow window;
        Presenter presenter;
        public WelcomeWindow()
        {
            InitializeComponent();
            window = new MainWindow();

            presenter = new Presenter(window);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            window.OpenFile();
            if (!string.IsNullOrEmpty(window.fileName))
            {
                this.Close();
                window.Show();
            }
        }

        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            window.NewFile();

            if (!string.IsNullOrEmpty(window.fileName))
            {
                this.Close();
                window.Show();
            }
        }

        private void WelcomeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
