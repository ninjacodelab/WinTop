using System;
using System.Text;

namespace WinTop
{
    class Helper
    {
        public static string FormatAsPercentage(decimal decimalInput)
        {
            if (decimalInput < 0.0m) return "00.0%";
            return decimalInput < 10.0m ? $"0{decimalInput:F1}%" : $"{decimalInput:F1}%";
        }

        public static string GetBarGraphSegments(decimal percentageUsed)
        {
            int barLength = (int)percentageUsed / 5;
            if (barLength < 0) barLength = 0;
            var sb = new StringBuilder();
            sb.Append('|', barLength);
            sb.Append(' ', 20 - barLength);
            return sb.ToString();
        }

        public static TimeSpan GetProcessorUsage(ulong kernelModeTime, ulong userModeTime)
        {
            ulong seconds = (kernelModeTime + userModeTime) / 10000000;
            return TimeSpan.FromSeconds(seconds);
        }

        public static string GetSystemUptime(DateTime lastBoot)
        {
            TimeSpan uptime = DateTime.Now - lastBoot;
            return uptime.ToString(@"dd\.hh\:mm\:ss");
        }

        public static void ShowUsageHints(int height, int width)
        {
            string hints = "S: toggle stats | V: toggle group view | X: exit";
            //Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            int left = width - hints.Length - 1;
            int top = height - 1;
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            Console.SetCursorPosition(left, top);
            Console.Write(hints);
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void PrepareConsoleForExit()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
        }
    }
}
