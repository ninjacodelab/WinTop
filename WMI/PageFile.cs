using System.Management;

namespace WinTop.WMI
{
    class PageFile
    {
        public uint TotalSize { get; set; }
        public uint CurrentUsage { get; set; }
        public uint PeakUsage { get; set; }
        public decimal PercentageUsed { get; set; }

        public PageFile(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PageFileUsage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            foreach (var item in collection)
            {
                TotalSize = (uint)item["AllocatedBaseSize"];
                CurrentUsage = (uint)item["CurrentUsage"];
                PeakUsage = (uint)item["PeakUsage"];
                PercentageUsed = (decimal)CurrentUsage / TotalSize * 100;
            }
        }
    }
}
