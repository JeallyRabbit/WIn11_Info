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

namespace WIn11_Info
{
    /// <summary>
    /// Interaction logic for adminPassword.xaml
    /// </summary>
    public partial class adminPasswordForm : Window
    {
        public bool credentialsSuccess = false;
        public adminPasswordForm()
        {
            InitializeComponent();
            passwdBoxPassword.Focus();
            credentialsSuccess = false;
        }

        private void btnCredentialsOK_Click(object sender, RoutedEventArgs e)
        {
            credentialsSuccess = true;
            this.Close();
        }

        private void btnCredentialsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnCredentialsOK_Click(sender, e);
            }
        }
    }
}
