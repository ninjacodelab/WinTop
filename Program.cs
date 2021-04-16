using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using WinTop.WMI;
using Process = WinTop.WMI.Process;

namespace WinTop
{
    class Program
    {
        public static Processor Processor;

        static void Main(string[] args)
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            scope.Connect();
            Processor = new Processor(scope);
            bool runAgain = true;
            bool displayProcessesAsGroup = false;
            bool showStats = false;

            Console.CursorVisible = false;
            Console.Clear();

            do
            {
                GetAndDisplayProcessInformation(scope, displayProcessesAsGroup, showStats);
                if (Console.KeyAvailable == false)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    runAgain = ProcessInput(keyInfo.Key, ref displayProcessesAsGroup, ref showStats);
                }
            } while (runAgain);
        }

        // TODO: Refactor
        private static void GetAndDisplayProcessInformation(ManagementScope scope, bool displayProcessesAsGroup, bool showStats)
        {
            // TODO: Keep?
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Get console dimensions
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;

            // Get data from WMI
            Memory memory = new Memory(scope);
            PageFile pageFile = new PageFile(scope);

            // Write information gathered so far to the console
            Console.SetCursorPosition(0, 0);
            Processor.WriteToConsole(scope, 1, memory.NumOfProcesses);
            memory.WriteToConsole(2);
            pageFile.WriteToConsole(memory.LastBootTime, 3);
            Console.WriteLine();

            // Get process data from WMI
            Process process = new Process(scope);

            // Write process information to the console
            if (displayProcessesAsGroup)
                process.WriteToConsoleAsGroups(height, width);
            else
                process.WriteToConsoleAsList(height, width);

            // Show usage hints
            Helper.ShowUsageHints(height, width);

            // Show statistics
            if (showStats)
            {
                Console.SetCursorPosition(2, height - 1);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{stopwatch.ElapsedMilliseconds.ToString()} ms | X: {width} | Y: {height}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.SetCursorPosition(2, height - 1);
                Console.Write($"                             ");
            }
        }

        private static bool ProcessInput(ConsoleKey key, ref bool group, ref bool stats)
        {
            switch (key)
            {
                case ConsoleKey.X:
                    Helper.PrepareConsoleForExit();
                    return false;
                case ConsoleKey.V:
                    group = !group;
                    break;
                case ConsoleKey.S:
                    stats = !stats;
                    break;
            }

            return true;
        }
    }
}
