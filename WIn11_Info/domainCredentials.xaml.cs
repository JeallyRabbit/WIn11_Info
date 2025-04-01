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
using System.DirectoryServices.AccountManagement;

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
        public bool credentialsSuccess = false;

        private void txtBoxUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtBoxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, txtBoxDomainName.Text))
                {
                    // validate the credentials
                    bool isValid = pc.ValidateCredentials(txtBoxUserName.Text, passwordBoxCredentials.Password);
                    if (isValid)
                    {
                        credentialsSuccess = true;
                    }
                    else
                    {
                        MessageBox.Show("Wrong Credentials");
                    }
                }
                if (credentialsSuccess)
                {
                    this.Close();
                }
            }
            catch (PrincipalServerDownException ex)
            {
                MessageBox.Show($"Can't connect to domain: {txtBoxDomainName.Text}");
            }
            
            
        }

        private void btnCredentialsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
