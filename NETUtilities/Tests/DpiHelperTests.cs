using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfUtilities;

namespace Tests
{
    [TestClass]
    public class DpiHelperTests
    {
        [TestMethod]
        public void GetDpiByManagement()
        {
            var dpi = DpiHelper.GetDpiByGraphics();
        }
    }
}
