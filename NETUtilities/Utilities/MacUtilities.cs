using System.Management;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class MacUtilities
    {
        public static string GetMacByNetworkInterface()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var @interface in interfaces)
            {
                var mac = @interface.GetPhysicalAddress().ToString();
                if (mac.Length > 0)
                {
                    var result = Regex.Replace(mac, ".{2}", "$0:");
                    return result.Remove(result.Length - 1);
                }
            }

            return string.Empty;
        }

        public static string GetMacByWmi()
        {
            var isManagerRunning = false;

            try
            {
                var serviceController = new ServiceController("Winmgmt");
                isManagerRunning = serviceController.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                // do nothing
            }

            if (!isManagerRunning)
            {
                return null;
            }

            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();

            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if ((bool)mo["IPEnabled"])
                {
                    return mo["MacAddress"].ToString().ToUpper();
                }

                mo.Dispose();
            }

            return null;
        }
    }
}