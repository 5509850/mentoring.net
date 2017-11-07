using System;
using System.Runtime.InteropServices;

namespace Module_3.Library.Interoperating_with_Unmanaged_Code
{
    internal class PowerManagementInterop
    {
        [DllImport("powrprof.dll")]
        public static extern uint CallNtPowerInformation(
           PowerInformationLevel InformationLevel,
           IntPtr lpInputBuffer,
           int nInputBufferSize,
           out SystemPowerInformation spi,
           int nOutputBufferSize
       );

        [DllImport("PowrProf.dll", SetLastError = true)]
        public static extern uint CallNtPowerInformation(
        PowerInformationLevel InformationLevel,
        IntPtr lpInputBuffer,
        int nInputBufferSize,
        ref SystemBatteryState lpOutputBuffer,
        int nOutputBufferSize
        );

        [DllImport("PowrProf.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
