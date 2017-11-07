using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Module_3.Library.Interoperating_with_Unmanaged_Code
{
    [ComVisible(true)]
    [Guid("604E3E41-2658-4CD6-A13B-015F293FD563")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerState : IPowerState
    {
        private const uint statusSuccess = 0;        

        public SystemPowerInformation ShowSystemPowerInformation()
        {
            SystemPowerInformation spi;
            uint retval = PowerManagementInterop.CallNtPowerInformation(
                PowerInformationLevel.SystemPowerInformation,
                IntPtr.Zero,
                0,
                out spi,
                Marshal.SizeOf(typeof(SystemPowerInformation))
            );
            if (retval != statusSuccess)
            {
                throw new Exception("SystemPowerInformation not success");
            }
            return spi;
        }

        public SystemBatteryState GetSystemBatteryState()
        {
            var sbs = new SystemBatteryState();
            uint retval = PowerManagementInterop.CallNtPowerInformation(
                PowerInformationLevel.SystemBatteryState,
                IntPtr.Zero,
                0,
                ref sbs,
                Marshal.SizeOf(sbs));
            if (retval != statusSuccess)
            {
                throw new Exception("GetSystemBatteryState not success");
            }
            return sbs;
        }

        public bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent)
        {
            return PowerManagementInterop.SetSuspendState(hibernate, forceCritical, disableWakeEvent);
        }        

        public void CreateHibernateFile()
        {
            RunCommandConsole(@"/hibernate on");
        }

        public void DeleteHibernateFile()
        {
            RunCommandConsole(@"/hibernate off");
        }

        private void RunCommandConsole(string argument)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "powercfg.exe";
                p.StartInfo.Arguments = argument;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Verb = "runas";
                p.Start();
            }
            catch (Win32Exception ex)
            {
                var err = ex.Message;
            }           
        }
    }
}
