using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Module_3.Task_1.Interoperating_with_Unmanaged_Code
{
    internal static class Power
    {
        internal static PowerManagementNativeMethods.SystemPowerCapabilities
            GetSystemPowerCapabilities()
        {
            PowerManagementNativeMethods.SystemPowerCapabilities powerCap;

            uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
              PowerManagementNativeMethods.PowerInformationLevel.SystemPowerCapabilities,
              IntPtr.Zero, 0, out powerCap,
              (UInt32)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemPowerCapabilities))
              );

            if (retval == CoreNativeMethods.StatusAccessDenied)
            {
                throw new UnauthorizedAccessException("error");
            }

            return powerCap;
        }

        internal static PowerManagementNativeMethods.SystemBatteryState GetSystemBatteryState()
        {
            PowerManagementNativeMethods.SystemBatteryState batteryState;

            uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
              PowerManagementNativeMethods.PowerInformationLevel.SystemBatteryState,
              IntPtr.Zero, 0, out batteryState,
              (UInt32)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemBatteryState))
              );

            if (retval == CoreNativeMethods.StatusAccessDenied)
            {
                throw new UnauthorizedAccessException("error");
            }

            return batteryState;
        }

        /// <summary>
        /// Registers the application to receive power setting notifications 
        /// for the specific power setting event.
        /// </summary>
        /// <param name="handle">Handle indicating where the power setting 
        /// notifications are to be sent.</param>
        /// <param name="powerSetting">The GUID of the power setting for 
        /// which notifications are to be sent.</param>
        /// <returns>Returns a notification handle for unregistering 
        /// power notifications.</returns>
        internal static int RegisterPowerSettingNotification(
            IntPtr handle, Guid powerSetting)
        {
            int outHandle = PowerManagementNativeMethods.RegisterPowerSettingNotification(
                handle,
                ref powerSetting,
                0);

            return outHandle;
        }
    }
}