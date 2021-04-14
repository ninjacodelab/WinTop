using System;
using System.Management;

namespace WinTop.WMI
{
    class Processor
    {
        public static string GetInfo(ManagementScope scope)
        {
            ObjectQuery cpuQuery = new ObjectQuery("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher(scope, cpuQuery);
            ManagementObjectCollection cpuCollection = cpuSearcher.Get();

            string cpuInfo = string.Empty;

            foreach (var c in cpuCollection)
            {
                cpuInfo = $"Name: {c["Name"]} ({c["NumberOfCores"]} Cores)";
            }

            return cpuInfo;
        }

        public static decimal GetUtilization(ManagementScope scope)
        {
            ObjectQuery cpuInfoQuery =
                new ObjectQuery("SELECT * FROM Win32_PerfFormattedData_Counters_ProcessorInformation");
            ManagementObjectSearcher cpuInfoSearcher = new ManagementObjectSearcher(scope, cpuInfoQuery);
            ManagementObjectCollection cpuInfoCollection = cpuInfoSearcher.Get();

            // TODO: There has to be a more accurate way to calculate CPU utilization
            int coreCount = 0;
            decimal combinedIdleTime = 0.0m;

            foreach (var i in cpuInfoCollection)
            {
                coreCount++;
                combinedIdleTime += Convert.ToDecimal(i["PercentIdleTime"]);
            }

            return 100.0m - (combinedIdleTime / coreCount);
        }
    }
}
