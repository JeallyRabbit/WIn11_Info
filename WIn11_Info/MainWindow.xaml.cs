using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
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
        bool lanReadingSuccess = false;
        bool wlanReadingSuccess = false;

        String dhcpRecord1 = "";
        String dhcpRecord2 = "";

        public MainWindow()
        {
            InitializeComponent();
            btnSetHostName.IsEnabled = false;
            lblDhcpRecord1.IsEnabled = false;
            btnDhcpRecord1.IsEnabled= false;
            lblDhcpRecord2.IsEnabled = false;
            btnDhcpRecord2.IsEnabled= false;

        }

        private void btnShowSN_Click(object sender, RoutedEventArgs e)
        {
            String sn=Tools.GetLocalSN();
            if(sn != "1")
            {
                btnShowSN.Content = "SN: " + sn;
            }
        }
        /*
        private void btnShowIP_Click(object sender, RoutedEventArgs e)
        {
            String ip=Tools.GetLocalIPAddress();
            btnShowIP.Content = ip;
        }
        */

        private void btnShowCPU_Click(object sender, RoutedEventArgs e)
        {
            String cpu=Tools.getCPU();
            txtBlockCpu.Text = cpu;
        }

        private void btnShowHostName_Click(object sender, RoutedEventArgs e)
        {
            btnShowHostName.Content=System.Net.Dns.GetHostName();
        }

        /*
        private void btnShowLAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String lan = Tools.GetLocalMac_Lan();
                if (lan != "")
                {
                    btnShowLAN.Content = "LAN: "+lan;
                    lanReadingSuccess = true;
                }
                else
                {
                    MessageBox.Show("Error reading MAC address");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        */

        /*
        private void btnShowWLAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String mac=Tools.GetLocalMac_Wlan();
                if(mac!="0")
                {
                    btnShowWLAN.Content = "WLAN: "+mac;
                    wlanReadingSuccess = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        */

        private void btnDhcpRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //btnShowLAN_Click(sender, e);
                //btnShowWLAN_Click(sender, e);
                if (lanReadingSuccess)
                {
                    lblDhcpRecord1.IsEnabled = true;
                    btnDhcpRecord1.IsEnabled = true;

                    dhcpRecord1 = "host " + System.Net.Dns.GetHostName().ToString() + "_LAN { hardware ethernet " +
                        Tools.GetLocalMac_Lan() + "; fixed-address " + txtBoxDhcpRecord.Text.ToString() + ";}\n";

                    //Difference in double underscore for label element
                    lblDhcpRecord1.Content = "host " + System.Net.Dns.GetHostName().ToString() + "__LAN { hardware ethernet " +
                        Tools.GetLocalMac_Lan() + "; fixed-address " + txtBoxDhcpRecord.Text.ToString() + ";}\n";
                }

                if(wlanReadingSuccess)
                {
                    lblDhcpRecord2.IsEnabled = true;
                    btnDhcpRecord2.IsEnabled = true;
                    dhcpRecord2= "host " + System.Net.Dns.GetHostName().ToString() + "_WLAN { hardware ethernet " +
                        Tools.GetLocalMac_Wlan() + "; fixed-address " + txtBoxDhcpRecord.Text.ToString() + ";}\n";

                    //Difference in double underscore for label element
                    lblDhcpRecord2.Content = "host " + System.Net.Dns.GetHostName().ToString() + "__WLAN { hardware ethernet " +
                        Tools.GetLocalMac_Wlan() + "; fixed-address " + txtBoxDhcpRecord.Text.ToString() + ";}\n";
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
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
                txtBoxHostName.FontWeight = FontWeights.Normal; txtBoxHostName.Focus();
            }
        }

        /*
        private void txtBoxHostNameKeyDown(object sender, KeyEventArgs e)
        {

        }
        */

        private void txtBoxHostNameKey_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSetHostName_Click(sender,e);
            }
        }

        private void txtBoxHostName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            int validate=Tools.ValidateNetbiosName(txtBoxHostName.Text);
            btnSetHostName.IsEnabled = false;
            if(validate == 0)
            {
                btnSetHostName.IsEnabled = true;
                if(ErrorPopup!=null)
                {
                    ErrorPopup.IsOpen = false;
                }
            }
            else if(validate == 1)
            {
                if(PopupText!=null && ErrorPopup!=null)
                {
                    PopupText.Text = "HostName can't be empty.";
                    ErrorPopup.IsOpen = true;
                }
                
            }
            else if(validate == 2)
            {
                if (PopupText != null && ErrorPopup != null)
                {
                    PopupText.Text = "Max. 15 characters.";
                    ErrorPopup.IsOpen = true;
                }
            }
            else if (validate == 3)
            {
                if (PopupText != null && ErrorPopup != null)
                {
                    PopupText.Text = "Only A-Z, a-z, 0-9 and '-'.";
                    ErrorPopup.IsOpen = true;
                }
            }
            else if (validate == 4)
            {
                if (PopupText != null && ErrorPopup != null)
                {
                    PopupText.Text = "HostName can't start with '-'.";
                    ErrorPopup.IsOpen = true;
                }
            }
            else if (validate == 5)
            {
                if (PopupText != null && ErrorPopup != null)
                {
                    PopupText.Text = "HostName can't be all digits.";
                    ErrorPopup.IsOpen = true;
                }
            }
            else if (validate == 6)
            {
                if (PopupText != null && ErrorPopup != null)
                {
                    PopupText.Text = "It is one of reserved names.";
                    ErrorPopup.IsOpen = true;
                }
            }

        }

        private void btnDhcpRecord1_Click(object sender, RoutedEventArgs e)
        {
            if(lblDhcpRecord1.IsVisible)
            {
                Clipboard.SetDataObject(dhcpRecord1);
            }
        }

        private void btnDhcpRecord2_Click(object sender, RoutedEventArgs e)
        {
            if (lblDhcpRecord2.IsVisible)
            {
                Clipboard.SetDataObject(dhcpRecord2);
            }
        }

        private void btnShowRam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnRam.Content = Tools.getRAM() + " GB";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnShowDisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBlockHardDisk.Text = Tools.getLocalDisk();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBoxNrInw_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBoxNrInw.Text == "Nr. Inw.")
            {
                txtBoxNrInw.Text = String.Empty;
                txtBoxNrInw.FontWeight = FontWeights.Normal;
            }
        }

        private void txtBoxId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBoxId.Text == "00000")
            {
                txtBoxId.Text = String.Empty;
                txtBoxId.FontWeight = FontWeights.Normal;
            }
        }

        private void btnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            Tools.exportToCsv(txtBoxNrInw.Text, txtBoxId.Text);
        }

        private void btnExportXML_Click(object sender, RoutedEventArgs e)
        {
            Tools.exportToXml(txtBoxNrInw.Text, txtBoxId.Text);
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            btnShowSN_Click(sender, e);
            btnShowCPU_Click(sender, e);
            btnShowHostName_Click(sender, e);
            //btnShowIP_Click(sender, e);
            //btnShowLAN_Click(sender, e);
            //btnShowWLAN_Click(sender, e);
            btnShowRam_Click(sender, e);
            btnShowDisk_Click(sender, e);
        }

        private void btnSetLocalAdmin_Click(object sender, RoutedEventArgs e)
        {
            Tools.setLocalAdmin();
        }

        private void btnSetLastUser_Click(object sender, RoutedEventArgs e)
        {
            Tools.setLastUser();
        }

        private void btnSaveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            Tools.saveToDatabase(Tools.GetLocalSN(),txtBoxNrInw.Text.ToString(),txtBoxId.Text.ToString());
        }


        private void btnShowNetwork_Click(object sender, RoutedEventArgs e)
        {
            Tools.btnShowNetwork_Click();
        }
    }
}