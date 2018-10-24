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

namespace ChangeManagerWPF.Models
{
    /// <summary>
    /// Interaction logic for PrivateKeyWindow.xaml
    /// </summary>
    public partial class PrivateKeyWindow : Window
    {
        Action<string, string> newChangeManager;
        string gitProject;

        public PrivateKeyWindow(Action<string, string> newChangeManager, string gitProject)
        {
            InitializeComponent();
            this.newChangeManager = newChangeManager;
            this.gitProject = gitProject;
        }

        private void privateKeyOK(object sender, RoutedEventArgs e)
        {
            if (privateKey.Password != null)
            {
                newChangeManager(gitProject, privateKey.Password);
                Close();

            }
        }

        private void privateKeyCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
