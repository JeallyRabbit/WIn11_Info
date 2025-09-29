using System.Runtime.InteropServices;
using System.Windows;


namespace WIn11_Info
{
    /// <summary>
    /// Interaction logic for ShowCpu.xaml
    /// </summary>
    /// 
    public partial class ShowCpu : Window
    {

        internal static class ConsoleAllocator
        {
            [DllImport(@"kernel32.dll", SetLastError = true)]
            static extern bool AllocConsole();

            [DllImport(@"kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport(@"user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SwHide = 0;
            const int SwShow = 5;


            public static void ShowConsoleWindow()
            {
                var handle = GetConsoleWindow();

                if (handle == IntPtr.Zero)
                {
                    AllocConsole();
                }
                else
                {
                    ShowWindow(handle, SwShow);
                }
            }

            public static void HideConsoleWindow()
            {
                var handle = GetConsoleWindow();

                ShowWindow(handle, SwHide);
            }
        }


        public ShowCpu()
        {
            InitializeComponent();

            /////////////
            ///console for debugging
            //ConsoleAllocator.ShowConsoleWindow();
            ///////////////

            List<string> supportedFeatures = new List<string>();

            supportedFeatures = Tools.getAdvancedCpuFeatures();


            this.SizeToContent = SizeToContent.WidthAndHeight;

            lblCPUName.Content += Tools.getCpu() + "\n";
            lblCPUName.Content += "Supported features:\n";
            double startHeight = lblCPUName.Height;
            foreach (string feature in supportedFeatures)
            {
                lblCPUName.Content += "   - " + feature + "\n";
                lblCPUName.Height = lblCPUName.Height + startHeight;
            }




        }
    }
}
