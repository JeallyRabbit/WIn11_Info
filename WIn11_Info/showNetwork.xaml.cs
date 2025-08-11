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
    /// Interaction logic for showNetwork.xaml
    /// </summary>
    public partial class showNetwork : Window
    {
        public showNetwork()
        {
            InitializeComponent();
            Title = System.Net.Dns.GetHostName();
            lblIP.Content = "Ip: " + Tools.GetLocalIPAddress();
            lblMacLan.Content = "Mac Lan: " + Tools.GetLocalMac_Lan();
            lblMacWlan.Content = "Mac Wlan: " + Tools.GetLocalMac_Wlan();

            try
            {

                string dhcpRecordLan = "host " + System.Net.Dns.GetHostName().ToString() + "__LAN { hardware ethernet " +
                    Tools.GetLocalMac_Lan() + "; fixed-address " + this.lblIP.Content + ";}\n";

                lblDhcpLan.Content = dhcpRecordLan;



                string dhcpRecordWlan = "host " + System.Net.Dns.GetHostName().ToString() + "__WLAN { hardware ethernet " +
                    Tools.GetLocalMac_Wlan() + "; fixed-address " + this.lblIP.Content + ";}\n";

                lblDhcpWlan.Content = dhcpRecordWlan;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void lblDhcpLan_myClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(lblDhcpLan.Content);

        }

        private void lblDhcpWlan_myClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(lblDhcpWlan.Content);
        }
    }
}
