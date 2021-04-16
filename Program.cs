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
        static void Main(string[] args)
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            scope.Connect();
            Console.Clear();
            bool runAgain = true;
            bool displayProcessesAsGroup = false;

            ConsoleKeyInfo keyInfo;
            do
            {
                GetAndDisplayProcessInformation(scope, displayProcessesAsGroup);
                Thread.Sleep(2000);
                keyInfo = Console.ReadKey(true);
                runAgain = ProcessInput(keyInfo.Key, ref displayProcessesAsGroup);
            } while (runAgain);
        }

        // TODO: Refactor
        private static void GetAndDisplayProcessInformation(ManagementScope scope, bool displayProcessesAsGroup)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Get console dimensions
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();

            // Get data from WMI
            Processor processor = new Processor(scope);
            Memory memory = new Memory(scope);
            PageFile pageFile = new PageFile(scope);

            // Write information gathered so far to the console
            processor.WriteToConsole(scope, 1, memory.NumOfProcesses);
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

            Console.SetCursorPosition(2, height - 1);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{stopwatch.ElapsedMilliseconds.ToString()} ms");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static bool ProcessInput(ConsoleKey key, ref bool group)
        {
            switch (key)
            {
                case ConsoleKey.X:
                    Helper.PrepareConsoleForExit();
                    return false;
                case ConsoleKey.V:
                    @group = !@group;
                    break;
            }

            return true;
        }
    }
}
