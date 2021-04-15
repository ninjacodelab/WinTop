using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinTop.WMI;

namespace WinTop
{
    class Helper
    {
        public static void DisplayProcessListHeader(int consoleWidth)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            string columnHeaders = "      ID  PRI          MEM   THRD           TIME  COMMAND";
            StringBuilder sb = new StringBuilder();
            sb.Append(columnHeaders);
            sb.Append(' ', consoleWidth - columnHeaders.Length - 1);
            Console.WriteLine(sb.ToString());
            // Cleanup
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void DisplayProcessGroupHeader(int consoleWidth)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            string columnHeaders = "  MEMORY USED   PROCESS GROUP";
            StringBuilder sb = new StringBuilder();
            sb.Append(columnHeaders);
            sb.Append(' ', consoleWidth - columnHeaders.Length - 1);
            Console.WriteLine(sb.ToString());
            // Cleanup
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void DisplayCpuInformation(decimal utilization, string name, int numOfCores, int numOfProcesses)
        {
            int rowPosition = 1;
            int colPosition = 0;
            if (utilization < 0) utilization = 0;
            string barGraph = GetBarGraphSegments(utilization);

            // Show CPU usage
            Console.SetCursorPosition(2, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("CPU");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowPosition);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowPosition);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowPosition);
            Console.WriteLine($"{FormatAsPercentage(utilization)}]");

            // Show CPU name and number of cores
            Console.SetCursorPosition(34, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Name:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(40, rowPosition);
            Console.Write(name);
            colPosition = name.Length + 41;
            Console.SetCursorPosition(colPosition, rowPosition);
            Console.Write($"({numOfCores} Cores)");

            // Show the number of tasks
            colPosition += 12;
            Console.SetCursorPosition(colPosition, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Tasks: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{numOfProcesses} total");

            // Cleanup
            Console.SetCursorPosition(0,rowPosition + 1);
        }

        public static void DisplayMemoryInformation(Memory memory)
        {
            int rowPosition = 2;
            if (memory.PercentageUsed < 0) memory.PercentageUsed = 0;
            string barGraph = GetBarGraphSegments(memory.PercentageUsed);

            // Show memory usage
            Console.SetCursorPosition(2, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Mem");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowPosition);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowPosition);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowPosition);
            Console.WriteLine($"{FormatAsPercentage(memory.PercentageUsed)}]");

            // Show amount of physical memory detected
            Console.SetCursorPosition(34, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Size:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(40, rowPosition);
            Console.Write($"{memory.TotalGb:F1} GB ");

            // Cleanup
            Console.SetCursorPosition(0, rowPosition + 1);
        }

        public static void DisplayPagingInformation(PageFile pageFile, Memory memory)
        {
            int rowPosition = 3;
            if (pageFile.PercentageUsed < 0) pageFile.PercentageUsed = 0;
            string barGraph = GetBarGraphSegments(pageFile.PercentageUsed);

            // Show page file usage
            Console.SetCursorPosition(2, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Pge");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowPosition);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowPosition);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowPosition);
            Console.WriteLine($"{FormatAsPercentage(pageFile.PercentageUsed)}]");

            // Show the system uptime
            Console.SetCursorPosition(34, rowPosition);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Uptime:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(42, rowPosition);
            Console.Write($"{GetSystemUptime(memory.LastBootTime)}");

            // Cleanup
            Console.SetCursorPosition(0, rowPosition + 1);
        }

        public static void DisplayProcessList(List<ProcessInfo> processList, int consoleHeight, int consoleWidth)
        {
            Helper.DisplayProcessListHeader(consoleWidth);

            var topProcesses = processList.Select(x => x).OrderBy(x => x.RamUsed).Reverse().Take(consoleHeight - 7);
            foreach (var process in topProcesses)
            {
                Console.WriteLine($"{process.Pid,8}{process.Priority,5}{process.RamUsedMb,10:F1} MB" +
                                  $"{process.ThreadCount,7}{process.ProcessorUsed,15}  {process.Name,-40}");
            }

        }

        public static void DisplayProcessesByGroups(List<ProcessGroupInfo> processGroups, int consoleHeight, int consoleWidth)
        {
            Helper.DisplayProcessGroupHeader(consoleWidth);

            foreach (var processGroup in processGroups.OrderBy(x => x.TotalRamUsed).Reverse().Take(consoleHeight - 7))
            {
                Console.WriteLine($"{processGroup.TotalRamUsedMb,10} MB   {processGroup.Name,-50}");
            }
        }

        public static string FormatAsPercentage(decimal decimalInput)
        {
            if (decimalInput < 0.0m) return "00.0%";
            return decimalInput < 10.0m ? $"0{decimalInput:F1}%" : $"{decimalInput:F1}%";
        }

        public static string GetBarGraphSegments(decimal percentageUsed)
        {
            int barLength = (int)percentageUsed / 5;
            if (barLength < 0) barLength = 0;
            var sb = new StringBuilder();
            sb.Append('|', barLength);
            sb.Append(' ', 20 - barLength);
            return sb.ToString();
        }

        public static TimeSpan GetProcessorUsage(ulong kernelModeTime, ulong userModeTime)
        {
            ulong seconds = (kernelModeTime + userModeTime) / 10000000;
            return TimeSpan.FromSeconds(seconds);
        }

        public static string GetSystemUptime(DateTime lastBoot)
        {
            TimeSpan uptime = DateTime.Now - lastBoot;
            return uptime.ToString(@"dd\.hh\:mm\:ss");
        }
    }
}
