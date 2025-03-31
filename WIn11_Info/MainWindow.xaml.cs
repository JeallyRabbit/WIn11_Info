using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WIn11_Info
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnShowSN_Click(object sender, RoutedEventArgs e)
        {
            String sn=Tools.GetLocalSN();
            if(sn != "1")
            {
                btnShowSN.Content = "SN: " + sn;
            }
        }

        private void btnShowIP_Click(object sender, RoutedEventArgs e)
        {
            String ip=Tools.GetLocalIPAddress();
            btnShowIP.Content = ip;
        }

        private void btnShowCPU_Click(object sender, RoutedEventArgs e)
        {
            String cpu=Tools.getCPU();
            txtBlockCpu.Text = cpu;
            //btnShowCPU.Content = Tools.getCPU();
        }

        private void btnShowHostName_Click(object sender, RoutedEventArgs e)
        {
            btnShowHostName.Content=System.Net.Dns.GetHostName();
        }

        private void btnShowLAN_Click(object sender, RoutedEventArgs e)
        {
            String lan = Tools.GetLocalMac_Lan();
            if(lan!="")
            {
                btnShowLAN.Content = lan;
            }
            else
            {
                MessageBox.Show("Error reading MAC address");
            }
        }

        private void btnShowWLAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String mac=Tools.GetLocalMac_Wlan2();
                if(mac!="0")
                {
                    btnShowWLAN.Content = mac;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnDhcpRecord_Click(object sender, RoutedEventArgs e)
        {
            btnShowLAN_Click(sender,e);
            btnShowWLAN_Click(sender, e);
            lblDhcpRecord1.Content="host "+ System.Net.Dns.GetHostName().ToString()+"__LAN { hardware ethernet "+
                btnShowLAN.Content+"; fixed-address "+txtBoxDhcpRecord.Text.ToString() + ";}\n";

            lblDhcpRecord2.Content = "host " + System.Net.Dns.GetHostName().ToString() + "__LAN { hardware ethernet " +
                btnShowWLAN.Content + "; fixed-address " + txtBoxDhcpRecord.Text.ToString() + ";}\n";
        }

        private void btnSetHostName_Click(object sender, RoutedEventArgs e)
        {
            Tools.setLocalHostName(txtBoxHostName.Text);
        }

        private void txtBoxHostName_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtBoxHostName.Text=="HostName")
            {
                txtBoxHostName.Text=String.Empty;
            }
        }
    }
}