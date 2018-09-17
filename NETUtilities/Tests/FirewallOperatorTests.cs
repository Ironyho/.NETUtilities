using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Tests
{
    [TestClass]
    public class FirewallOperatorTests
    {
        [TestMethod]
        public void AddException()
        {
            FirewallOperator.AddException(@"C:\Program Files (x86)\Seewo\EasiNote5\EasiNote5_5.1.8.54557\Main\EasiNote.Cloud.exe");
        }
    }
}