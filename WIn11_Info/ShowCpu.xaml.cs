using iTin.Core.Hardware.Common;
using iTin.Hardware.Specification;
using iTin.Hardware.Specification.Cpuid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Runtime.Intrinsics.X86;
using System.Printing;


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
            ConsoleAllocator.ShowConsoleWindow();
            ///////////////

            List<string> supportedFeatures = new List<string>();
            var cpuid = CPUID.Instance;
            

            if (!cpuid.IsAvailable)
            {
                Console.WriteLine("Instrukcja CPUID nie jest dostępna na tym procesorze.");
                return;
            }

            Console.WriteLine("=== Informacje o procesorze z CPUID ===");

            // 1. Vendor ID
            QueryPropertyResult manufacturerQueryResult = cpuid.Leafs.GetProperty(LeafProperty.BasicInformation.Manufacturer);
            if (manufacturerQueryResult.Success)
            {
                Console.WriteLine($@" > Manufacturer: {manufacturerQueryResult.Result.Value}");
            }

            // 2. Full CPU Name (Brand tring)
            var brandQuerryResult = cpuid.Leafs.GetProperty(LeafProperty.ProcessorBrandString.ProcessorName);
            var brandQuerryResult1 = cpuid.Leafs.GetProperty(LeafProperty.ProcessorBrandString1.ProcessorNameContinued);
            var brandQuerryResult2 = cpuid.Leafs.GetProperty(LeafProperty.ProcessorBrandString2.ProcessorNameContinued);
            if (brandQuerryResult.Success && brandQuerryResult1.Success && brandQuerryResult2.Success)
            {
                Console.WriteLine($"Brand String {brandQuerryResult.Result.Value}"+$"{brandQuerryResult1.Result.Value}"
                    + $"{brandQuerryResult2.Result.Value}");
            }

            // 3. Processor SerialNumber
            var processorSnResult=cpuid.Leafs.GetProperty(LeafProperty.ProcessorSerialNumber.SerialNumber);
            if(processorSnResult.Success)
            {
                Console.WriteLine($"Processor SN: {processorSnResult.Result.Value}");
            }

            // 4. Cores amount
            
            Console.WriteLine($"Threads: {Environment.ProcessorCount}");
            

            //5 Cache Size
            var cacheSizeQuerry = cpuid.Leafs.GetProperty(LeafProperty.DeterministicCacheParameters.CacheSize);

            ////lahf/sahf, SSE3, SSE4.1/4.2,  EM64T, AES, AVX512, FMA3/4
            ///

            //6 lahf/sahf
            var lahfsahfResult = cpuid.Leafs.GetProperty(LeafProperty.ExtendedProcessorInfoAndFeatures.LAHF_SAHF);
            if (lahfsahfResult.Success)
            {
                Console.WriteLine($"lahfsahfResult : {lahfsahfResult.Result.Value}");
                supportedFeatures.Add("lahf/sahf");
            }

            //7 SSE3
            var sse3Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE3);
            if (sse3Result.Success)
            {
                Console.WriteLine($"sse3Result : {sse3Result.Result.Value}");
                supportedFeatures.Add("sse3");
            }

            //8 SSE4.1
            var sse41Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE41);
            if (sse41Result.Success)
            {
                Console.WriteLine($"sse41Result : {sse41Result.Result.Value}");
                supportedFeatures.Add("sse4.1");
            }

            //9 SSE4.2
            var sse42Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE42);
            if (sse42Result.Success)
            {
                Console.WriteLine($"sse42Result : {sse42Result.Result.Value}");
                supportedFeatures.Add("sse4.2");
            }

            //10 EM64T
            var em64tResult = cpuid.Leafs.GetProperty(LeafProperty.ExtendedProcessorInfoAndFeatures.I64);
            if (em64tResult.Success)
            {
                // here put asm
                // Equivalent to: mov eax, 0x80000001; xor ecx, ecx; cpuid
                var (eax, ebx, ecx, edx) = X86Base.CpuId(unchecked((int)0x80000001), 0);

                // Test EDX bit 29: Long Mode (Intel 64 / EM64T / AMD64)
                bool longMode = (edx & (1 << 29)) != 0;
                if(longMode)
                {
                    supportedFeatures.Add("Intel64/EM64T");
                }
                Console.WriteLine($"EAX=0x{eax:X8} EBX=0x{ebx:X8} ECX=0x{ecx:X8} EDX=0x{edx:X8}");
                Console.WriteLine($"Intel 64 / EM64T supported: {longMode}");

                //


                //Console.WriteLine($"em64tResult : {em64tResult.Result.Value}"); // False
                var architecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                Console.WriteLine($"ArchitectureL {architecture}"); // -> AMD64
            }

            

            //11 AES
            var aesResult =cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.AES);
            if (aesResult.Success)
            {
                Console.WriteLine($"aesResult: {aesResult.Result.Value}");
                supportedFeatures.Add("AES");
            }

            //12 AVX512 - we use "_F" since it: "expands most 32-bit and 64-bit based AVX instructions"
            var avx512Result = cpuid.Leafs.GetProperty(LeafProperty.ExtendedFeatures.AVX512_F);
            if (avx512Result.Success)
            {
                Console.WriteLine($"avx512Result: {avx512Result.Result.Value}");
                supportedFeatures.Add("avx512");
            }

            //13 FMA3/4
            var fmaResult = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.FMA);
            if (fmaResult.Success)
            {
                Console.WriteLine($"fmaResult: {fmaResult.Result.Value}");
                supportedFeatures.Add("fma");
            }
            

            // 5. Stepping
            var stepping = cpuid.Leafs.GetProperty(LeafProperty.SystemOnChipInformation.SteppingID); 
                //cpuid[Leafs.BasicInformation][0].GetProperty(Properties.SteppingId).Value;
            Console.WriteLine($"Stepping: {stepping}");


            
            lblCPUName.Content+= Tools.getCpu();
            foreach(string feature in supportedFeatures)
            {
                lblCPUName.Content += feature + "\n";
            }




        }
    }
}
