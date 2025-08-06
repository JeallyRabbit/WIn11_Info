// For saving to csv
using CsvHelper;
using Microsoft.Win32;
// For saving to sql database
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
// Allows to use Win32 Classes
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
// For saving to xml
using System.Xml.Linq;



public class ComputerUnit
{
    public string Sn { get; set; }
    public string Ip { get; set; }
    public string MacLan { get; set; }
    public string MacWlan { get; set; }
    public string Cpu { get; set; }
    public string HostName { get; set; }
    public string HardDisk { get; set; }
    public string Ram { get; set; }
    public string Nr { get; set; }
    public string Id { get; set; }

    // Constructor to populate properties using methods or constants
    public ComputerUnit(string sn, string ip, string macLan, string macWlan, string cpu,
                      string hostName, string hardDisk, string ram, string nr = null, string id = null)
    {
        Sn = sn;
        Ip = ip;
        MacLan = macLan;
        MacWlan = macWlan;
        Cpu = cpu;
        HostName = hostName;
        HardDisk = hardDisk;
        Ram = ram;
        Nr = nr;
        Id = id;
    }
}

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
            return "0";
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

        public static string GetLocalMac_Wlan()
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

        public static string getRAM()
        {
            string ram = "";
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * from Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                double Ram_Bytes = (Convert.ToDouble(obj["TotalPhysicalMemory"]));
                Ram_Bytes /= 1073741824;
                Ram_Bytes = Math.Ceiling(Ram_Bytes);
                ram = (Ram_Bytes).ToString();
            }
            return ram;
        }


        public static string getLocalDisk()
        {
            string drive_info = "";
            ManagementObjectSearcher harddisk =
                new ManagementObjectSearcher("SELECT * from win32_DiskDrive");
            foreach (ManagementObject obj in harddisk.Get())
            {
                drive_info += obj["Model"];
                Int64 disk_bytes = Convert.ToInt64(obj["Size"]);
                disk_bytes /= 1073741824;
                drive_info += " Size: " + (disk_bytes).ToString() + "GB";
            }
            return drive_info;
        }

        public static int exportToCsv(String NR="",String ID="")
        {
            String Sn = GetLocalSN();
            String Ip = GetLocalIPAddress();
            String MacLan = GetLocalMac_Lan();
            String MacWlan = GetLocalMac_Wlan();
            String Cpu = getCPU();
            String HostName = System.Net.Dns.GetHostName();
            String HardDisk = getLocalDisk();
            String Ram = getRAM();
            String Nr = NR;
            String Id = ID;

            if (HostName != "")
            {
                ComputerUnit unit = new ComputerUnit(Sn, Ip, MacLan, MacWlan, Cpu, HostName, HardDisk, Ram, Nr, Id);
                String fileName = HostName + "_Report.csv";
                using (var writer = new StreamWriter(fileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<ComputerUnit>();
                    csv.NextRecord(); // Move to next line after header
                    csv.WriteRecord(unit);
                    csv.NextRecord(); // Ensure newline
                }


            }
            else
            {//can't create file - invalid name
                MessageBox.Show("Can't create report - invalid name (hostName)");
            }


            return 1;
        }

        public static int exportToXml(String NR="", String ID="")
        {

            String Sn = GetLocalSN();
            String Ip = GetLocalIPAddress();
            String MacLan = GetLocalMac_Lan();
            String MacWlan = GetLocalMac_Wlan();
            String Cpu = getCPU();
            String HostName = System.Net.Dns.GetHostName();
            String HardDisk = getLocalDisk();
            String Ram = getRAM();
            String Nr = NR;
            String Id = ID;

            if (HostName != "")
            {
                ComputerUnit unit = new ComputerUnit(Sn, Ip, MacLan, MacWlan, Cpu, HostName, HardDisk, Ram, Nr, Id);
                String fileName = HostName + "_Report.xml";
                
                XDocument doc = new XDocument(
                    new XElement("Computer",
                        new XElement("HostName",HostName),
                        new XElement("SN",Sn),
                        new XElement("IP",Ip),
                        new XElement("MacLan",MacLan),
                        new XElement("MacWlan",MacWlan),
                        new XElement("Cpu",Cpu),
                        new XElement("Ram",Ram),
                        new XElement("HardDisk",HardDisk),
                        new XElement("Nr",Nr),
                        new XElement("ID",Id)
                    )
                );

                doc.Save( fileName );
                MessageBox.Show("Saved to: " + fileName);


            }
            else
            {//can't create file - invalid name
                MessageBox.Show("Can't create report - invalid name (hostName)");
            }



            return 1;
        }

        public static bool setLocalAdmin()
        {
            string userName = "Administrator";
            string password = "";
            adminPasswordForm passwordForm = new adminPasswordForm();
            passwordForm.ShowDialog();

            if (passwordForm.IsActive == false)
            {
                if (passwordForm.credentialsSuccess == false) { return false; }
                password = passwordForm.passwdBoxPassword.Password;
            }

            if(password!="")
            {

                // Set password
                RunCommand($"net user {userName} {password}");

                // Enable account
                RunCommand($"net user {userName} /active:yes");

                Console.WriteLine("Administrator account updated.");
            

                static void RunCommand(string command)
                {
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas" // Run as administrator
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        string output = process.StandardOutput.ReadToEnd();
                        Console.WriteLine(output);
                    }
                }
        }
            return true;
        }

        public static void setLastUser()
        {

            string userName = "";
            string domainName = "";
            lastUserForm userForm = new lastUserForm();
            userForm.ShowDialog();

            if (userForm.IsActive == false)
            {
                if (userForm.lastUserSuccess == false) { return; }
                userName=userForm.txtBoxUserName.Text;
                domainName=userForm.txtBoxDomainName.Text;
            }
            if(userName!="" && domainName!="")
            {
                const string registryEntry = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\";
                string onDisplayKey = "LastLoggedOnDisplayName";
                string onSamUserKey = "LastLoggedOnSAMUser";
                string onUserKey = "LastLoggedOnUser";


                Registry.SetValue(registryEntry , onDisplayKey, userName);
                Registry.SetValue(registryEntry , onSamUserKey, domainName+"\\"+userName);
                Registry.SetValue(registryEntry , onUserKey, domainName + "\\" + userName);

                MessageBox.Show("Successfully set last user.");

            }
            else
            {
                MessageBox.Show("Invalid userName or domainName");
            }


        }

        static void ensureDataBaseExists(string masterConnStr, string dbName)
        {
            string checkDbSql = $@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}')
            BEGIN
                CREATE DATABASE {dbName}
            END";

            using var connection = new SqlConnection(masterConnStr);
            using var command = new SqlCommand(checkDbSql, connection);
            connection.Open(); 
            command.ExecuteNonQuery();
        }


        static void ensureTableExists(string dbConnStr,string tabName)
        {
            string createTableSql = $@"
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tabName}') BEGIN
                CREATE TABLE PCs (
                    ID INT IDENTITY PRIMARY KEY,
                     Sn NVARCHAR(30),
                     Ip NVARCHAR(15),
                     MacLan NVARCHAR(17),
                     MacWlan NVARCHAR(17),
                     Cpu NVARCHAR(100),
                     HostName NVARCHAR(100),
                     HardDisk NVARCHAR(100),
                     Ram NVARCHAR(100),
                     Nr NVARCHAR(10),
                     Id_Number NVARCHAR(20)
                )
            END";

            using var connection = new SqlConnection(dbConnStr);
            using var command = new SqlCommand(createTableSql, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }

        static bool isInDatabase(string dbConnStr, string tabName,string sn="", string NR = "000000", string Id_Numberd = "000000")
        {
            string result = "";
            string selectSql = $"SELECT * FROM {tabName} WHERE Sn = '{sn}' ";
            using (var connection = new SqlConnection(dbConnStr))
            using (var command = new SqlCommand(selectSql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        MessageBox.Show($"PC: {sn} is already in database");
                        return true;
                    }
                }
            }
                

            return false;
        }

            static void insertPC(string dbConnStr, string tabName, string NR="000000", string Id_Numberd="000000")
        {



            string insertSql = $"INSERT INTO {tabName} "+
                @"(Sn, Ip,MacLan,MacWlan ,Cpu ,HostName ,HardDisk ,Ram ,Nr ,Id_Number) 
    VALUES (@Sn, @Ip,@MacLan,@MacWlan ,@Cpu ,@HostName ,@HardDisk ,@Ram ,@Nr ,@Id_Number)";

            using var connection = new SqlConnection(dbConnStr);
            using var command = new SqlCommand(insertSql, connection);

            String Sn = GetLocalSN();//
            String Ip = GetLocalIPAddress();//
            String MacLan = GetLocalMac_Lan();//
            String MacWlan = GetLocalMac_Wlan();//
            String Cpu = getCPU();//
            String HostName = System.Net.Dns.GetHostName();//
            String HardDisk = getLocalDisk();
            String Ram = getRAM();//
            String Nr = NR;//
            String IdNumber = Id_Numberd;//


            command.Parameters.AddWithValue("@Sn", Sn);
            command.Parameters.AddWithValue("@Ip", Ip);
            command.Parameters.AddWithValue("@MacLan", MacLan);
            command.Parameters.AddWithValue("@MacWlan", MacWlan);
            command.Parameters.AddWithValue("@HostName", HostName);
            command.Parameters.AddWithValue("@HardDisk", HardDisk);
            command.Parameters.AddWithValue("@Ram", Ram);
            command.Parameters.AddWithValue("@Nr", Nr);
            command.Parameters.AddWithValue("@Id_Number", IdNumber);
            command.Parameters.AddWithValue("@Cpu", Cpu);
            connection.Open();

            int insertResult = command.ExecuteNonQuery();
            if (insertResult==1)
            {
                MessageBox.Show("User inserted successfully.");
            }
            else
            {
                MessageBox.Show($"Affected: {insertResult} rows.");
            }

            
        }


        public static void saveToDatabase(String SN="",String NR = "", String ID = "")
        {

            string server = "localhost\\SQLEXPRESS01"; // or  SQL Server name
            string userName = "UserName";
            string Password = "Password";
            string dbName = "master";
            string tabName = "PCs";
            //"Server=localhost\\SQLEXPRESS01;Database=master;Trusted_Connection=True;"
            string masterConnectionString = $"Server={server};Database=master;Integrated Security=true;";
            string appConnectionString = $"Server={server};Database={dbName};Integrated Security=true;";

            ensureDataBaseExists(masterConnectionString, dbName);
            ensureTableExists(appConnectionString,tabName);
            if(isInDatabase(appConnectionString, tabName, SN, NR, ID)==false)
            {
                insertPC(appConnectionString,tabName,NR,ID);
            }
            

            

        }

    }
}
