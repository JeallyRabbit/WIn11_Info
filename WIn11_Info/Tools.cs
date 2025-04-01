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
using System.Diagnostics;
using System.Text.RegularExpressions;


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

        public static bool setLocalHostName(string newName)
        {
            string domainName = "";
            string domainUser = "";
            string domainPassword = "";
            domainCredentials credentialsForm=new domainCredentials();
            credentialsForm.ShowDialog();

            if (credentialsForm.IsActive == false)
            {
                if (credentialsForm.credentialsSuccess == false) { return false; }
                domainUser = credentialsForm.txtBoxUserName.Text.ToString();
                domainPassword = credentialsForm.passwordBoxCredentials.Password.ToString();
                domainName = credentialsForm.txtBoxDomainName.Text.ToString();
            }



            string script = $@"
            $newName = '{newName}'
            $domain = '{domainName}'
            $username = '{domainUser}'
            $password = ConvertTo-SecureString '{domainPassword}' -AsPlainText -Force
            $credential = New-Object System.Management.Automation.PSCredential ($username, $password);
            $ErrorActionPreference = 'Stop';
            Rename-Computer -NewName $newName -DomainCredential $credential -Force";


            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true

            };
            using (Process process = new Process { StartInfo = processInfo })
            {

                process.Start();

                // Read the output (optional)
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                
                // print the status of command
                if (process.ExitCode != 0) {
                    MessageBox.Show("Exit code = " + process.ExitCode + "\n" +
                        "error " + error+"\n"+"Output "+output);

                }
                else
                {
                    MessageBoxResult dialogResult = MessageBox.Show("Reboot is required, reboot now ?", "Reboot", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        Process.Start("ShutDown", "/r");
                    }
                }
                

            }
            return true;
        }

        public static int ValidateNetbiosName(string name)
        {
            // Rule 1: Not null or empty
            if (string.IsNullOrWhiteSpace(name))
            { 
                return 1;
            }
            // Rule 2: Max 15 characters
            if (name.Length > 15)
                return 2;

            // Rule 3: Only A-Z, a-z, 0-9, and hyphen
            if (!Regex.IsMatch(name, @"^[A-Za-z0-9\-]+$"))
                return 3;

            // Rule 4: Cannot start or end with hyphen
            if (name.StartsWith("-") || name.EndsWith("-"))
                return 4;

            // Rule 5: Cannot be all digits
            if (Regex.IsMatch(name, @"^\d+$"))
                return 5;

            // Rule 6: Reserved names
            string[] reserved = new[]
            {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            if (reserved.Contains(name.ToUpper()))
                return 6;

            // Passed all checks
            return 0;
        }

    }
}
