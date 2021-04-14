using System;
using System.Management;

namespace WinTop.WMI
{
    class Memory
    {
        public decimal PctUsed { get; set; }
        public ulong TotalKb { get; set; }
        public int NumOfProcesses { get; set; }

        public void GetInfo(ManagementScope scope)
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

                PctUsed = pctUsedRam;
                TotalKb = totalRamInKb;
                NumOfProcesses = Convert.ToInt32(m["NumberOfProcesses"]);
            }
        }
    }
}
