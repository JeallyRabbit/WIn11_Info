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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class lastUserForm : Window
    {
        public string domainName = "";
        public string userName = "";
        public bool lastUserSuccess = true;
        public lastUserForm()
        {
            InitializeComponent();
            txtBoxUserName.Focus();
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            txtBoxDomainName.Text = System.Net.Dns.GetHostName();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            lastUserSuccess = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lastUserSuccess = false;
            this.Close();
        }
    }
}
