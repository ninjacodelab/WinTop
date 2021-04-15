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
            // Get console dimensions
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;

            //Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();

            // Processor
            var cpuName = string.Empty;
            var numOfCores = 0;
            Processor.GetInfo(scope, out cpuName, out numOfCores);
            var cpuUtilization = Processor.GetUtilization(scope);

            // Memory
            Memory memory = new Memory(scope);
            Helper.DisplayCpuInformation(cpuUtilization, cpuName, numOfCores, memory.NumOfProcesses);
            Helper.DisplayMemoryInformation(memory);

            // Paging space
            PageFile pageFile = new PageFile(scope);
            Helper.DisplayPagingInformation(pageFile, memory);

            // Add a blank line between cpu/memory/paging section and the process section
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
