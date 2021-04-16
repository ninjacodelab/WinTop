using System;
using System.Management;

namespace WinTop.WMI
{
    class PageFile
    {
        public uint TotalSize { get; set; }
        public uint CurrentUsage { get; set; }
        public uint PeakUsage { get; set; }
        public decimal PercentageUsed { get; set; }

        public PageFile(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PageFileUsage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            foreach (var pageFile in collection)
            {
                TotalSize = (uint)pageFile["AllocatedBaseSize"];
                CurrentUsage = (uint)pageFile["CurrentUsage"];
                PeakUsage = (uint)pageFile["PeakUsage"];
                PercentageUsed = (decimal)CurrentUsage / TotalSize * 100;
            }
        }

        public void WriteToConsole(DateTime lastBootTime, int rowNum)
        {
            if (rowNum < 0) rowNum = 0;
            if (PercentageUsed < 0) PercentageUsed = 0;
            string barGraph = Helper.GetBarGraphSegments(PercentageUsed);

            // Show page file usage
            Console.SetCursorPosition(2, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Pge");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowNum);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowNum);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowNum);
            Console.WriteLine($"{Helper.FormatAsPercentage(PercentageUsed)}]");

            // TODO: Move the following into the memory object?
            // Show the system uptime
            Console.SetCursorPosition(34, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Uptime:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(42, rowNum);
            Console.Write($"{Helper.GetSystemUptime(lastBootTime)}");

            // Cleanup
            Console.SetCursorPosition(0, rowNum + 1);
        }
    }
}
