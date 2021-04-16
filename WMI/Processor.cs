using System;
using System.Management;

namespace WinTop.WMI
{
    class Processor
    {
        public string Name { get; set; }
        public int NumOfCores { get; set; }
        public decimal Utilization { get; set; }

        public Processor(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = searcher.Get();

            foreach (var processor in collection)
            {
                Name = Convert.ToString(processor["Name"]);
                NumOfCores = Convert.ToInt16(processor["NumberOfCores"]);
            }
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

        public void WriteToConsole(ManagementScope scope, int rowNum, int numOfProcesses)
        {
            Utilization = GetUtilization(scope);

            int colPosition = 0;
            if (rowNum < 0) rowNum = 0;
            if (Utilization < 0) Utilization = 0;
            string barGraph = Helper.GetBarGraphSegments(Utilization);

            // Show CPU usage
            Console.SetCursorPosition(2, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("CPU");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(5, rowNum);
            Console.WriteLine("[");
            Console.SetCursorPosition(6, rowNum);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(barGraph);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(26, rowNum);
            Console.WriteLine($"{Helper.FormatAsPercentage(Utilization)}]");

            // Show CPU name and number of cores
            Console.SetCursorPosition(34, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Name:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(40, rowNum);
            Console.Write(Name);
            colPosition = Name.Length + 41;
            Console.SetCursorPosition(colPosition, rowNum);
            Console.Write($"({NumOfCores} Cores)");

            // TODO: Move the following into the memory object?
            // Show the number of tasks
            colPosition += 12;
            Console.SetCursorPosition(colPosition, rowNum);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Tasks: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{numOfProcesses} total");

            // Cleanup
            Console.SetCursorPosition(0, rowNum + 1);
        }
    }
}
