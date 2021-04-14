namespace WinTop
{
    class ProcessGroupInfo
    {
        public string Name { get; set; }
        public ulong TotalRamUsed { get; set; }
        public ulong TotalRamUsedKb => TotalRamUsed / 1024;
        public ulong TotalRamUsedMb => TotalRamUsedKb / 1024;
    }
}
