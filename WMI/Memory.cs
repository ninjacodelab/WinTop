using System;
using System.Management;

namespace WinTop.WMI
{
    class Memory
    {
        public decimal PercentageUsed { get; set; }
        public ulong TotalKb { get; set; }
        public double TotalMb => TotalKb / 1024.0;
        public double TotalGb => TotalKb / 1048576.0;
        public int NumOfProcesses { get; set; }
        public DateTime LastBootTime { get; set; }

        public Memory(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            foreach (var item in collection)
            {
                ulong freeRamInKb = (ulong)item["FreePhysicalMemory"];
                ulong totalRamInKb = (ulong)item["TotalVisibleMemorySize"];
                decimal pctFreeRam = Math.Round((decimal)freeRamInKb / totalRamInKb * 100, 1);
                decimal pctUsedRam = 100 - pctFreeRam;

                PercentageUsed = pctUsedRam;
                TotalKb = totalRamInKb;
                NumOfProcesses = Convert.ToInt32(item["NumberOfProcesses"]);
                LastBootTime = ManagementDateTimeConverter.ToDateTime(item["LastBootUpTime"].ToString());
            }
        }

        public void WriteToConsole(int rowNum)
        {
            if (rowNum < 0) rowNum = 0;
            if (PercentageUsed < 0) PercentageUsed = 0;
            string barGraph = Helper.GetBarGraphSegments(PercentageUsed);

            // Show memory usage
            Console.SetCursorPosition(2, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Mem");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowNum);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowNum);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowNum);
            Console.WriteLine($"{Helper.FormatAsPercentage(PercentageUsed)}]");

            // Show amount of physical memory detected
            Console.SetCursorPosition(34, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Size:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(40, rowNum);
            Console.Write($"{TotalGb:F1} GB ");

            // Cleanup
            Console.SetCursorPosition(0, rowNum + 1);
        }
    }
}
