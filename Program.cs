using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using WinTop.WMI;

namespace WinTop
{
    class Program
    {
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

            // List of single processes
            List<ProcessInfo> processList = Process.GetAll(scope);
            //Helper.DisplayProcessList(processList, height, width);

            // List of grouped processes
            List<ProcessGroupInfo> processGroups = Process.GetProcessGroups(scope, processList);
            Helper.DisplayProcessesByGroups(processGroups, height, width);
        }
    }
}
