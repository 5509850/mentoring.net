using System.Runtime.InteropServices;

namespace Module_3.Library.Interoperating_with_Unmanaged_Code
{
    public struct SystemBatteryState
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool AcOnLine;
        [MarshalAs(UnmanagedType.I1)]
        public bool BatteryPresent;
        [MarshalAs(UnmanagedType.I1)]
        public bool Charging;
        [MarshalAs(UnmanagedType.I1)]
        public bool Discharging;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
        public bool[] Spare1;
        public uint MaxCapacity;
        public uint RemainingCapacity;
        public uint Rate;
        public uint EstimatedTime;
        public uint DefaultAlert1;
        public uint DefaultAlert2;
    }
}
