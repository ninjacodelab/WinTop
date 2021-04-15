using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace WinTop.WMI
{
    class Process
    {
        public static List<ProcessInfo> GetAll(ManagementScope scope)
        {
            ObjectQuery processQuery = new ObjectQuery("SELECT * FROM Win32_Process");
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher(scope, processQuery);
            var list = (from ManagementObject mo in processSearcher.Get()
                select mo).ToList();

            List<ProcessInfo> processList = new List<ProcessInfo>();

            foreach (var entry in list)
            {
                processList.Add(new ProcessInfo
                {
                    Name = (string)entry["Name"],
                    Pid = (uint)entry["ProcessId"],
                    Priority = (uint)entry["Priority"],
                    RamUsed = (ulong)entry["WorkingSetSize"],
                    ThreadCount = (uint)entry["ThreadCount"],
                    ProcessorUsed = Helper.GetProcessorUsage((ulong)entry["KernelModeTime"], (ulong)entry["UserModeTime"])
                });
            }

            return processList;
        }

        public static List<ProcessGroupInfo> GetProcessGroups(ManagementScope scope, List<ProcessInfo> processList)
        {
            return processList
                .GroupBy(process => process.Name)
                .Select(group => new ProcessGroupInfo
                {
                    Name = group.First().Name,
                    TotalRamUsed = group.Aggregate(0UL, (a, t) => a + t.RamUsed)
                }).ToList();
        }
    }
}
