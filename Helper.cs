using System;
using System.Globalization;
using System.Text;

namespace WinTop
{
    class Helper
    {
        public static string DisplayAsBarGraph(decimal percentageUsed)
        {
            int barLength = (int)percentageUsed / 5;
            if (barLength < 0) barLength = 0;
            var sb = new StringBuilder();
            sb.Append('[');
            sb.Append('|', barLength);
            sb.Append(' ', 20 - barLength);
            if (percentageUsed < 0.0m) percentageUsed = 0.0m;
            sb.Append(percentageUsed < 10.0m ? " " : string.Empty);
            sb.Append(percentageUsed.ToString("N1", CultureInfo.CurrentCulture));
            sb.Append("%]");
            return sb.ToString();
        }

        public static void DisplayProcessListHeader(int consoleWidth)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            string columnHeaders = "      ID  PRI          MEM   THRD           TIME  COMMAND";
            StringBuilder sb = new StringBuilder();
            sb.Append(columnHeaders);
            sb.Append(' ', consoleWidth - columnHeaders.Length - 1);
            Console.WriteLine(sb.ToString());
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static TimeSpan GetProcessorUsage(ulong kernelModeTime, ulong userModeTime)
        {
            ulong seconds = (kernelModeTime + userModeTime) / 10000000;
            return TimeSpan.FromSeconds(seconds);
        }

        //public static string GetProcessorUsageAsString(ulong kernelModeTime, ulong userModeTime)
        //{
        //    ulong processorTime = (kernelModeTime + userModeTime) / 10000000;
        //    ulong days = processorTime / 86400;
        //    ulong hours = (processorTime - (days * 86400)) / 3600;
        //    ulong minutes = (processorTime - (days * 86400) - (hours * 3600)) / 60;
        //    ulong seconds = processorTime - (days * 86400) - (hours * 3600) - (minutes * 60);
        //    return $"{days:D2}.{hours:D2}:{minutes:D2}:{seconds:D2}";
        //}
    }
}
