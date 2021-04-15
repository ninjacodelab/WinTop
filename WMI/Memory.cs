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
            ObjectQuery osQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, osQuery);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (var m in queryCollection)
            {
                ulong freeRamInKb = (ulong)m["FreePhysicalMemory"];
                ulong totalRamInKb = (ulong)m["TotalVisibleMemorySize"];
                decimal pctFreeRam = Math.Round((decimal)freeRamInKb / totalRamInKb * 100, 1);
                decimal pctUsedRam = 100 - pctFreeRam;

                PercentageUsed = pctUsedRam;
                TotalKb = totalRamInKb;
                NumOfProcesses = Convert.ToInt32(m["NumberOfProcesses"]);
                LastBootTime = ManagementDateTimeConverter.ToDateTime(m["LastBootUpTime"].ToString());
            }
        }
    }
}
