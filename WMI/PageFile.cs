using System.Management;

namespace WinTop.WMI
{
    class PageFile
    {
        public ulong TotalSize { get; set; }
        public ulong CurrentUsage { get; set; }
        public ulong PeakUsage { get; set; }

        public static string GetInfo(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PageFileUsage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            string info = string.Empty;

            foreach (var item in collection)
            {
                info = $"Name: {item["Name"]} ({item["NumberOfCores"]} Cores)";
            }

            return info;
        }
    }
}
