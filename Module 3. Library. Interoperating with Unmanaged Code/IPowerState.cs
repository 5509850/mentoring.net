using System;
using System.Runtime.InteropServices;

namespace Module_3.Library.Interoperating_with_Unmanaged_Code
{
    [ComVisible(true)]
    [Guid("E1753DD8-00E0-4DF7-93EB-B509B7C8F4BF")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerState
    {
        SystemPowerInformation ShowSystemPowerInformation();

        SystemBatteryState GetSystemBatteryState();

        bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        void CreateHibernateFile();

        void DeleteHibernateFile();
    }
}
