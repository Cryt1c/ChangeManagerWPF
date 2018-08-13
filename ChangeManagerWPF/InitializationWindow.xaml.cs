using ChangeManagerWPF.Models;
using System;
using System.Collections.Generic;
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

namespace ChangeManagerWPF
{
    /// <summary>
    /// Interaction logic for InitializationWindow.xaml
    /// </summary>
    public partial class InitializationWindow : Window
    {
        private Action<string, string> newChangeManager;
        private Action<string, string> useChangeManager;

        public InitializationWindow(Action<string, string> newChangeManager, Action<string, string> useChangeManager)
        {
            this.newChangeManager = newChangeManager;
            this.useChangeManager = useChangeManager;
            InitializeComponent();
        }

        private void initUse(object sender, RoutedEventArgs e)
        {
            useChangeManager(initAddress.Text, initGitProject.Text);
        }

        private void initNew(object sender, RoutedEventArgs e)
        {
            PrivateKeyWindow pKWnd = new PrivateKeyWindow(newChangeManager, initGitProject.Text);
            pKWnd.Show();
        }
    }
}
