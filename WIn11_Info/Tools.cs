using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// Allows to use Win32 Classes
using System.Management;
using System.Net.NetworkInformation;
using System.Windows;



namespace WIn11_Info
{
    internal class Tools
    {
        public static String GetLocalIPAddress()
        {
            
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static String GetLocalSN()
        {

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");

            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                {
                    if (data.Name.ToString().StartsWith("SerialN"))
                    {
                        searcher.Dispose();
                        return data.Value.ToString();
                    }
                }
            }
            searcher.Dispose();
            return "1";
        }

        public static String getCPU()
        {
            String cpu = "";
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT Name from Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                cpu += obj["Name"];
            }

            return cpu;
        }

        public static String GetLocalMac_Lan()
        {
            String addr = "";
            foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (n.Name.StartsWith("Ethernet"))
                {
                    addr = n.GetPhysicalAddress().ToString();
                    break;
                }
            }
            String lan = "";
            for (int i = 0; i < addr.Length; i++)
            {
                if (i % 2 == 0 && i > 0)
                {
                    lan += ":";
                }
                lan += addr[i];

            }
            return lan;
        }

        public static string GetLocalMac_Wlan2()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter");
            ManagementObjectCollection adapters = searcher.Get();

            foreach (ManagementObject adapter in adapters)
            {
                /*
                MessageBox.Show("Nazwa: " + adapter["Name"] + " Typ interfejsu: " + adapter["AdapterType"]
                    + " Status: " + adapter["NetConnectionStatus"] + " MAC: " + adapter["MACAddress"]);
                */
                if (adapter["Name"].ToString().Contains("Wi-Fi") &&
                    (adapter["AdapterType"].ToString().Contains("802.3")) || adapter["AdapterType"].ToString().Contains("802.11"))
                {
                    return adapter["MACAddress"].ToString();
                }
            }
            return "0";
        }
    }
}
