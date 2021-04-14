using System;

namespace WinTop
{
    class ProcessInfo
    {
        public TimeSpan ProcessorUsed { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public uint Pid { get; set; }
        public uint Ppid { get; set; }
        public uint Priority { get; set; }
        public ulong RamUsed { get; set; }
        public double RamUsedKb => RamUsed / 1024.0;
        public double RamUsedMb => RamUsedKb / 1024.0;
        public uint ThreadCount { get; set; }
    }
}
