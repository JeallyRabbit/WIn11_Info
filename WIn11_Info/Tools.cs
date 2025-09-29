// For saving to csv
using CsvHelper;
using iTin.Hardware.Specification;
using iTin.Hardware.Specification.Cpuid;
using Microsoft.Win32;
//For reading from JSON
using Newtonsoft.Json;
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
using System.Runtime.Intrinsics.X86;
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

    public class DbSettings
    {
        public string ServerName { get; set; }

        public string TableName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }

    }


    internal class Tools
    {
        public static String GetLocalIPAddress()
        {

            string localIP = "0.0.0.0";
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    return "0.0.0.0";
                }
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


        public static List<string> getAdvancedCpuFeatures()
        {
            List<String> features = new List<String>();
            var cpuid = CPUID.Instance;


            if (!cpuid.IsAvailable)
            {
                return [];
            }


            //1 lahf/sahf
            var lahfsahfResult = cpuid.Leafs.GetProperty(LeafProperty.ExtendedProcessorInfoAndFeatures.LAHF_SAHF);
            if (lahfsahfResult.Success)
            {
                //Console.WriteLine($"lahfsahfResult : {lahfsahfResult.Result.Value}");
                features.Add("LAHF_SAHF");
            }

            //2 SSE3
            var sse3Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE3);
            if (sse3Result.Success)
            {
                //Console.WriteLine($"sse3Result : {sse3Result.Result.Value}");
                features.Add("sse3");
            }

            //3 SSE4_1
            var sse41Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE41);
            if (sse41Result.Success)
            {
                features.Add("SSE4_1");
            }

            //4 SSE4_1
            var sse42Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE42);
            if (sse42Result.Success)
            {
                features.Add("SSE4_1");
            }

            //5 EM64T
            var em64tResult = cpuid.Leafs.GetProperty(LeafProperty.ExtendedProcessorInfoAndFeatures.I64);
            if (em64tResult.Success)
            {
                // Equivalent to: mov eax, 0x80000001; xor ecx, ecx; cpuid
                var (eax, ebx, ecx, edx) = X86Base.CpuId(unchecked((int)0x80000001), 0);

                // Test EDX bit 29: Long Mode (Intel 64 / EM64T / AMD64)
                bool longMode = (edx & (1 << 29)) != 0;
                if (longMode)
                {
                    features.Add("Intel64_EM64T");
                }
            }



            //6 AES
            var aesResult = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.AES);
            if (aesResult.Success)
            {
                features.Add("AES");
            }

            //7 AVX512 - we use "_F" since it: "expands most 32-bit and 64-bit based AVX instructions"
            var avx512Result = cpuid.Leafs.GetProperty(LeafProperty.ExtendedFeatures.AVX512_F);
            if (avx512Result.Success)
            {
                features.Add("avx512");
            }

            //8 FMA3/4
            var fmaResult = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.FMA);
            if (fmaResult.Success)
            {
                features.Add("fma");
            }


            return features;
        }

        public static void showCpu()
        {

            ShowCpu showCpu = new ShowCpu();
            showCpu.ShowDialog();

            if (showCpu.IsActive == false)
            {
                return;
            }
        }

        public static String getCpu()
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

                try
                {
                    if (adapter["Name"] != null && adapter["AdapterType"] != null)
                    {
                        if (adapter["Name"].ToString().Contains("Wi-Fi") &&
                        (adapter["AdapterType"].ToString().Contains("802.3")) || adapter["AdapterType"].ToString().Contains("802.11"))
                        {

                            return adapter["MACAddress"].ToString();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            return "";
        }

        public static bool setLocalHostName(string newName)
        {
            string domainName = "";
            string domainUser = "";
            string domainPassword = "";
            domainCredentials credentialsForm = new domainCredentials();
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

                // Read the output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();


                // print the status of command
                if (process.ExitCode != 0)
                {
                    MessageBox.Show("Exit code = " + process.ExitCode + "\n" +
                        "error " + error + "\n" + "Output " + output);

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

        public static int exportToCsv(String NR = "", String ID = "")
        {
            String Sn = GetLocalSN();
            String Ip = GetLocalIPAddress();
            String MacLan = GetLocalMac_Lan();
            String MacWlan = GetLocalMac_Wlan();
            String Cpu = getCpu();
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

                MessageBox.Show("Saved to: " + fileName);
            }
            else
            {//can't create file - invalid name
                MessageBox.Show("Can't create report - invalid name (hostName)");
            }


            return 1;
        }

        public static int exportToXml(String NR = "", String ID = "")
        {

            String Sn = GetLocalSN();
            String Ip = GetLocalIPAddress();
            String MacLan = GetLocalMac_Lan();
            String MacWlan = GetLocalMac_Wlan();
            String Cpu = getCpu();
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
                        new XElement("HostName", HostName),
                        new XElement("SN", Sn),
                        new XElement("IP", Ip),
                        new XElement("MacLan", MacLan),
                        new XElement("MacWlan", MacWlan),
                        new XElement("Cpu", Cpu),
                        new XElement("Ram", Ram),
                        new XElement("HardDisk", HardDisk),
                        new XElement("Nr", Nr),
                        new XElement("ID", Id)
                    )
                );

                doc.Save(fileName);
                MessageBox.Show($"Saved to: {fileName}");


            }
            else
            {//can't create file - invalid name
                MessageBox.Show("Can't create report - invalid name (hostname)");
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

            if (password != "")
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
                userName = userForm.txtBoxUserName.Text;
                domainName = userForm.txtBoxDomainName.Text;
            }
            if (userName != "" && domainName != "")
            {
                const string registryEntry = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\";
                string onDisplayKey = "LastLoggedOnDisplayName";
                string onSamUserKey = "LastLoggedOnSAMUser";
                string onUserKey = "LastLoggedOnUser";


                Registry.SetValue(registryEntry, onDisplayKey, userName);
                Registry.SetValue(registryEntry, onSamUserKey, domainName + "\\" + userName);
                Registry.SetValue(registryEntry, onUserKey, domainName + "\\" + userName);

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


        static void ensureTableExists(string dbConnStr, string tabName)
        {
            string createTableSql = "";


            createTableSql = $@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tabName}') BEGIN
                CREATE TABLE {tabName} (
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
                     Id_Number NVARCHAR(20),
                        LAHF_SAHF NVARCHAR(20),
                        SSE3 NVARCHAR(20),
                        SSE4_1 NVARCHAR(20),
                        SSE4_2 NVARCHAR(20),
                        EM64T NVARCHAR(20),
                        AES NVARCHAR(20),
                        AVX512 NVARCHAR(20),
                        FMA3_4 NVARCHAR(20)
                )
            END";



            using var connection = new SqlConnection(dbConnStr);
            using var command = new SqlCommand(createTableSql, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }

        static bool isInDatabase(string dbConnStr, string tabName, string sn = "", string NR = "000000", string Id_Numberd = "000000")
        {
            string result = "";
            string selectSql = $"SELECT * FROM {tabName} WHERE Sn = '{sn}' ";
            using (var connection = new SqlConnection(dbConnStr))
            using (var command = new SqlCommand(selectSql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MessageBox.Show($"PC: {sn} is already in database");
                        return true;
                    }
                }
            }


            return false;
        }

        static void insertPC(string dbConnStr, string tabName, string NR = "000000", string Id_Numberd = "000000", List<string> features = null)
        {



            string insertSql = $"INSERT INTO {tabName} " +
                @"(Sn, Ip,MacLan,MacWlan ,Cpu ,HostName ,HardDisk ,Ram ,Nr ,Id_Number,
lahf_sahf,SSE3,SSE4_1 ,SSE4_2,EM64T,AES,AVX512,FMA3_4) 
    VALUES (@Sn, @Ip,@MacLan,@MacWlan ,@Cpu ,@HostName ,@HardDisk ,@Ram ,@Nr ,@Id_Number,
@lahf_sahf,@SSE3,@SSE4_1 ,@SSE4_2,@EM64T,@AES,@AVX512,@FMA3_4)";

            using var connection = new SqlConnection(dbConnStr);
            using var command = new SqlCommand(insertSql, connection);

            String Sn = GetLocalSN();
            String Ip = GetLocalIPAddress();
            String MacLan = GetLocalMac_Lan();
            String MacWlan = GetLocalMac_Wlan();
            String Cpu = getCpu();
            String HostName = System.Net.Dns.GetHostName();
            String HardDisk = getLocalDisk();
            String Ram = getRAM();
            String Nr = NR;
            String IdNumber = Id_Numberd;


            String lahf_sahf = "";
            String SSE3 = "";
            String SSE4_1 = "";
            String SSE4_2 = "";
            String EM64T = "";
            String AES = "";
            String AVX512 = "";
            String FMA3_4 = "";
            if (features.Count > 6)
            {
                lahf_sahf = features[0];
                SSE3 = features[1];
                SSE4_1 = features[2];
                SSE4_2 = features[3];
                EM64T = features[4];
                AES = features[5];
                AVX512 = features[6];
                FMA3_4 = features[7];
            }



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
            command.Parameters.AddWithValue("@lahf_sahf", lahf_sahf);
            command.Parameters.AddWithValue("@SSE3", SSE3);
            command.Parameters.AddWithValue("@SSE4_1", SSE4_1);
            command.Parameters.AddWithValue("@SSE4_2", SSE4_2);
            command.Parameters.AddWithValue("@EM64T", EM64T);
            command.Parameters.AddWithValue("@AES", AES);
            command.Parameters.AddWithValue("@AVX512", AVX512);
            command.Parameters.AddWithValue("@FMA3_4", FMA3_4);

            connection.Open();

            int insertResult = command.ExecuteNonQuery();
            if (insertResult == 1)
            {
                MessageBox.Show("PC inserted successfully.");
            }
            else
            {
                MessageBox.Show($"Affected: {insertResult} rows.");
            }


        }

        public static void loadDataBaseConf(string filePath, ref string server, ref string dbName, ref string tabName, ref string userName, ref string password)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var settings = JsonConvert.DeserializeObject<DbSettings>(json);

                if (settings != null)
                {
                    server = settings.ServerName;
                    dbName = settings.DatabaseName;
                    tabName = settings.TableName;
                    userName = settings.UserName;
                    password = settings.Password;

                }
                else
                {
                    MessageBox.Show($"Can't read data from: {filePath}");
                }
            }
            else
            {
                MessageBox.Show($"Can't find: {filePath}");
            }



        }
        public static void saveToDatabase(String SN = "", String NR = "", String ID = "")
        {
            string filePath = "dbSettings.json";
            string server = "";
            string userName = "";
            string password = "";
            string dbName = "";
            string tabName = "";
            loadDataBaseConf(filePath, ref server, ref dbName, ref tabName, ref userName, ref password);
            string masterConnectionString = $"Server={server};Database=master;Integrated Security=true;";
            string appConnectionString = $"Server={server};Database={dbName};Integrated Security=true;";

            ensureDataBaseExists(masterConnectionString, dbName);
            ensureTableExists(appConnectionString, tabName);


            if (isInDatabase(appConnectionString, tabName, SN, NR, ID) == false)
            {
                insertPC(appConnectionString, tabName, NR, ID, getAdvancedCpuFeatures());

            }





        }

        public static void btnShowNetwork_Click()
        {
            showNetwork networkForm = new showNetwork();
            networkForm.ShowDialog();

            if (networkForm.IsActive == false)
            {
                return;
            }
        }

    }
}
