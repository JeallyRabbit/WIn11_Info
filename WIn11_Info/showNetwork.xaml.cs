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
                    Tools.GetLocalMac_Lan() + "; fixed-address " + Tools.GetLocalIPAddress() + ";}\n";

                lblDhcpLan.Content = dhcpRecordLan;



                string dhcpRecordWlan = "host " + System.Net.Dns.GetHostName().ToString() + "__WLAN { hardware ethernet " +
                    Tools.GetLocalMac_Wlan() + "; fixed-address " + Tools.GetLocalIPAddress() + ";}\n";

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

        private void lblDhclWlanMouseEnter(object sender, MouseEventArgs e)
        {
            lblDhcpWlan.FontSize = 12;
        }

        private void lblDhclLanMouseEnter(object sender, MouseEventArgs e)
        {
            lblDhcpLan.FontSize = 12;
        }

        private void lblDhclLanMouseLeave(object sender, MouseEventArgs e)
        {
            lblDhcpLan.FontSize = 11;
        }

        private void lblDhcpWlanMouseLeave(object sender, MouseEventArgs e)
        {
            lblDhcpWlan.FontSize = 11;
        }
    }
}
