using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MacUtilitiesTests
    {
        [TestMethod]
        public void GetMacsByNetworkInterface()
        {
            var mac = Utilities.MacUtilities.GetMacsByNetworkInterface();
            Assert.AreEqual(mac.Count, 1);  // on Iron's surface
        }

        [TestMethod]
        public void GetMacsByWmi()
        {
            var macs = Utilities.MacUtilities.GetMacsByWmi();
            Assert.AreEqual(macs.Count, 1); // on Iron's surface
        }
    }
}