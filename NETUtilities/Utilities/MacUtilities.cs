using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace Utilities
{
    /// <summary>
    /// Provides some methods to access the media access control address (MAC).
    /// </summary>
    public static class MacUtilities
    {
        /// <summary>
        /// Get all MAC addresses through NetworkInterface.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetMacsByNetworkInterface()
        {
            var macs = new List<string>();

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var @interface in interfaces)
            {
                var up = @interface.OperationalStatus == OperationalStatus.Up;
                var loopback = @interface.NetworkInterfaceType == NetworkInterfaceType.Loopback;

                if (up && !loopback)
                {
                    var address = @interface.GetPhysicalAddress().ToString();

                    // insert ":" then remove the last ":"
                    var result = Regex.Replace(address, ".{2}", "$0:");
                    var mac = result.Remove(result.Length - 1);

                    macs.Add(mac);
                }
            }

            return macs;
        }

        /// <summary>
        /// Get all MAC addressess through WMI (Windows Management Instrumentation).
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetMacsByWmi()
        {
            var isManagerRunning = false;
            var macs = new List<string>();

            try
            {
                var serviceController = new ServiceController("Winmgmt");
                isManagerRunning = serviceController.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                // do nothing
            }

            if (isManagerRunning)
            {
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();

                foreach (var o in moc)
                {
                    var mo = (ManagementObject) o;

                    var address = mo["MacAddress"];
                    var enabled = (bool) mo["IPEnabled"];

                    if (address != null && enabled)
                    {
                        macs.Add(address.ToString().ToUpper());
                    }

                    mo.Dispose();
                }
            }

            return macs;
        }
    }
}