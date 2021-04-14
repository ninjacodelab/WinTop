using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

            // Processor
            var cpuInfo = Processor.GetInfo(scope);
            var cpuUtilization = Processor.GetUtilization(scope);
            Console.WriteLine($"CPU{Helper.DisplayAsBarGraph(cpuUtilization)}  {cpuInfo}");

            // Memory
            Memory memory = new Memory();
            memory.GetInfo(scope);
            Console.WriteLine($"Mem{Helper.DisplayAsBarGraph(memory.PctUsed)}  Size: {memory.TotalKb / OneMb:F1} GB" +
                              $"                                            Tasks: {memory.NumOfProcesses} total");

            // List of processes
            Helper.DisplayProcessListHeader();
            List<ProcessInfo> processList = WMI.Process.GetAll(scope);
            var top20Processes = processList.Select(x => x).OrderBy(x => x.RamUsed).Reverse().Take(20);
            foreach (var process in top20Processes)
            {
                Console.WriteLine($"{process.Pid,8}{process.Priority,5}{process.RamUsedMb,10:F1} MB" +
                                  $"{process.ThreadCount,7}{process.ProcessorUsed,15}  {process.Name,-40}");
            }

            // Group of processes
            //List<ProcessGroupInfo> processGroups = processList
            //    .GroupBy(process => process.Name)
            //    .Select(group => new ProcessGroupInfo {
            //        Name = group.First().Name,
            //        TotalRamUsed = group.Aggregate(0UL, (a,t) => a + t.RamUsed)
            //    }).ToList();

            //foreach (var processGroup in processGroups.OrderBy(x => x.TotalRamUsed).Reverse().Take(20))
            //{
            //    Console.WriteLine($"{processGroup.Name,40}{processGroup.TotalRamUsedMb,16} MB");
            //}
        }
    }
}
