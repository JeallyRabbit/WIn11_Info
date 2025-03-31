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
    /// Interaction logic for domainCredentials.xaml
    /// </summary>
    public partial class domainCredentials : Window
    {
        public domainCredentials()
        {
            InitializeComponent();
            txtBoxDomainName.Text = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName.ToString();
            passwordBoxCredentials.PasswordChar = '●';
            txtBoxDomainName.Focus();
        }
        public bool isPassing = false;

        private void txtBoxUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtBoxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            isPassing = true;
            this.Close();
        }

        private void btnCredentialsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
