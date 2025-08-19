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
            }

            //7 SSE3
            var sse3Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE3);
            if (sse3Result.Success)
            {
                Console.WriteLine($"sse3Result : {sse3Result.Result.Value}");
            }

            //8 SSE4.1
            var sse41Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE41);
            if (sse41Result.Success)
            {
                Console.WriteLine($"sse41Result : {sse41Result.Result.Value}");
            }

            //9 SSE4.2
            var sse42Result = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.SSE42);
            if (sse42Result.Success)
            {
                Console.WriteLine($"sse42Result : {sse42Result.Result.Value}");
            }

            //10 EM64T
            var em64tResult = cpuid.Leafs.GetProperty(LeafProperty.ExtendedProcessorInfoAndFeatures.I64);
            if (em64tResult.Success)
            {
                Console.WriteLine($"em64tResult : {em64tResult.Result.Value}"); // False
                var architecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                Console.WriteLine($"ArchitectureL {architecture}"); // -> AMD64
            }

            

            //11 AES
            var aesResult =cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.AES);
            if (aesResult.Success)
            {
                Console.WriteLine($"aesResult: {aesResult.Result.Value}");
            }

            //12 AVX512 - we use "_F" since it: "expands most 32-bit and 64-bit based AVX instructions"
            var avx512Result = cpuid.Leafs.GetProperty(LeafProperty.ExtendedFeatures.AVX512_F);
            if (avx512Result.Success)
            {
                Console.WriteLine($"avx512Result: {avx512Result.Result.Value}");
            }

            //13 FMA3/4
            var fmaResult = cpuid.Leafs.GetProperty(LeafProperty.ProcessorInfoAndFeatures.Features.FMA);
            if (fmaResult.Success)
            {
                Console.WriteLine($"fmaResult: {fmaResult.Result.Value}");
            }
            /*(
             3. Family
            var family = cpuid[Leafs.BasicInformation][0].GetProperty(Properties.Family).Value;
            Console.WriteLine($"Family: {family}");

            // 4. Model
            var model = cpuid[Leafs.BasicInformation][0].GetProperty(Properties.Model).Value;
            Console.WriteLine($"Model: {model}");

            // 5. Stepping
            var stepping = cpuid[Leafs.BasicInformation][0].GetProperty(Properties.SteppingId).Value;
            Console.WriteLine($"Stepping: {stepping}");

            // 6. Liczba rdzeni fizycznych
            var physicalCores = cpuid[Leafs.ProcessorTopology][0].GetProperty(Properties.PhysicalCoresPerPackage).Value;
            Console.WriteLine($"Fizyczne rdzenie: {physicalCores}");

            // 7. Liczba wątków logicznych
            var logicalCores = cpuid[Leafs.ProcessorTopology][0].GetProperty(Properties.LogicalCoresPerPackage).Value;
            Console.WriteLine($"Logiczne wątki: {logicalCores}");

            // 8–10. Cache L1, L2, L3
            var l1 = cpuid[Leafs.CacheParameters]
                .SelectMany(sl => sl)
                .Where(sl => sl.GetProperty(Properties.CacheLevel).Value.ToString() == "1")
                .Select(sl => sl.GetProperty(Properties.CacheSize).Value)
                .FirstOrDefault();
            Console.WriteLine($"Cache L1: {l1} KB");

            var l2 = cpuid[Leafs.CacheParameters]
                .SelectMany(sl => sl)
                .Where(sl => sl.GetProperty(Properties.CacheLevel).Value.ToString() == "2")
                .Select(sl => sl.GetProperty(Properties.CacheSize).Value)
                .FirstOrDefault();
            Console.WriteLine($"Cache L2: {l2} KB");

            var l3 = cpuid[Leafs.CacheParameters]
                .SelectMany(sl => sl)
                .Where(sl => sl.GetProperty(Properties.CacheLevel).Value.ToString() == "3")
                .Select(sl => sl.GetProperty(Properties.CacheSize).Value)
                .FirstOrDefault();
            Console.WriteLine($"Cache L3: {l3} KB");
            */




        }
    }
}
