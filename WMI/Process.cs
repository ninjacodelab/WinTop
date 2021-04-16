using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace WinTop.WMI
{
    class Process
    {
        public List<ProcessInfo> Processes { get; set; }
        public List<ProcessGroupInfo> ProcessGroups { get; set; }

        public Process(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            var list = (from ManagementObject mo in searcher.Get()
                select mo).ToList();

            Processes = new List<ProcessInfo>();

            foreach (var entry in list)
            {
                Processes.Add(new ProcessInfo
                {
                    Name = (string)entry["Name"],
                    Pid = (uint)entry["ProcessId"],
                    Priority = (uint)entry["Priority"],
                    RamUsed = (ulong)entry["WorkingSetSize"],
                    ThreadCount = (uint)entry["ThreadCount"],
                    ProcessorUsed = Helper.GetProcessorUsage((ulong)entry["KernelModeTime"], (ulong)entry["UserModeTime"])
                });
            }
        }

        public List<ProcessGroupInfo> GetProcessGroups()
        {
            return Processes
                .GroupBy(process => process.Name)
                .Select(group => new ProcessGroupInfo
                {
                    Name = group.First().Name,
                    TotalRamUsed = group.Aggregate(0UL, (a, t) => a + t.RamUsed)
                }).ToList();
        }

        public void WriteToConsoleAsList(int consoleHeight, int consoleWidth)
        {
            DisplayProcessListHeader(consoleWidth);
            var topProcesses = Processes.Select(x => x).OrderBy(x => x.RamUsed).Reverse().Take(consoleHeight - 7);

            foreach (var process in topProcesses)
            {
                Console.WriteLine($"{process.Pid,8}{process.Priority,5}{process.RamUsedMb,10:F1} MB" +
                                  $"{process.ThreadCount,7}{process.ProcessorUsed,15}  {process.Name,-40}");
            }
        }

        public void WriteToConsoleAsGroups(int consoleHeight, int consoleWidth)
        {
            DisplayProcessGroupHeader(consoleWidth);
            ProcessGroups = GetProcessGroups();

            foreach (var processGroup in ProcessGroups.OrderBy(x => x.TotalRamUsed).Reverse().Take(consoleHeight - 7))
            {
                Console.WriteLine($"{processGroup.TotalRamUsedMb,10} MB   {processGroup.Name,-50}");
            }
        }

        public void DisplayProcessListHeader(int consoleWidth)
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

        public void DisplayProcessGroupHeader(int consoleWidth)
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
    }
}
