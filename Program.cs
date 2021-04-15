using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using WinTop.WMI;

namespace WinTop
{
    class Program
    {
        private const double OneMb = 1048576.0;

        static void Main(string[] args)
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            scope.Connect();
            Console.Clear();

            do
            {
                GetAndDisplayProcessInformation(scope);
                Thread.Sleep(2000);
            } while (true);//Console.ReadKey(true).Key != ConsoleKey.X);
        }

        // TODO: Refactor
        private static void GetAndDisplayProcessInformation(ManagementScope scope)
        {
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;

            Console.SetCursorPosition(width - 1, height - 1);
            Console.Write("X");

            //Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();

            // Processor
            var cpuInfo = Processor.GetInfo(scope);
            var cpuUtilization = Processor.GetUtilization(scope);

            // Memory
            Memory memory = new Memory();
            memory.GetInfo(scope);
            Console.WriteLine($"  CPU{Helper.DisplayAsBarGraph(cpuUtilization)}  {cpuInfo}  Tasks: {memory.NumOfProcesses} total");
            Console.WriteLine($"  Mem{Helper.DisplayAsBarGraph(memory.PctUsed)}  Size: {memory.TotalKb / OneMb:F1} GB");

            // Paging space
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PageFileUsage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            string info = string.Empty;
            uint totalSize = 0;
            uint currentUsage = 0;

            foreach (var item in collection)
            {
                info = $"Total: {item["AllocatedBaseSize"]} Current: {item["CurrentUsage"]} Peak: {item["PeakUsage"]})";
                totalSize = (uint) item["AllocatedBaseSize"];
                currentUsage = (uint) item["CurrentUsage"];
            }

            decimal pagefilePctUsed = (decimal)currentUsage / totalSize * 100;

            TimeSpan upTime = DateTime.Now - memory.LastBootTime;
            Console.WriteLine($"  Pge{Helper.DisplayAsBarGraph(pagefilePctUsed)}  Uptime: {upTime:dd\\.hh\\:mm\\:ss}");

            Console.WriteLine();

            // List of processes
            Helper.DisplayProcessListHeader(width);
            List<ProcessInfo> processList = WMI.Process.GetAll(scope);
            var topProcesses = processList.Select(x => x).OrderBy(x => x.RamUsed).Reverse().Take(height - 7);
            foreach (var process in topProcesses)
            {
                Console.WriteLine($"{process.Pid,8}{process.Priority,5}{process.RamUsedMb,10:F1} MB" +
                                  $"{process.ThreadCount,7}{process.ProcessorUsed,15}  {process.Name,-40}");
            }

            //List<ProcessGroupInfo> processGroups = Process.GetProcessGroups(scope, processList);

            //foreach (var processGroup in processGroups.OrderBy(x => x.TotalRamUsed).Reverse().Take(height - 7))
            //{
            //    Console.WriteLine($"{processGroup.Name,40}{processGroup.TotalRamUsedMb,16} MB");
            //}
        }
    }
}
