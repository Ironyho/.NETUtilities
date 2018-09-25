using System;
using System.IO;
using System.Linq;
using NetFwTypeLib;

namespace Utilities
{
    /// <summary>
    /// Provides methods to add\remove\check firewall exceptions.
    /// </summary>
    public class FirewallOperator
    {
        /// <summary>
        /// Determines whether the firewall exception of the specified program file exists.
        /// </summary>
        /// <param name="fileName">The full path of the specified program file.</param>
        /// <returns></returns>
        public static bool ExceptionExists(string fileName)
        {
            var nameWithEx = Path.GetFileName(fileName);

            var cmd = $"{FirewallCmd} show rule name ={nameWithEx} verbose";
            var output = CmdRunner.ExecuteWithOutput(cmd);

            if (!string.IsNullOrEmpty(output))
            {
                if (output.Contains(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds firewall exception for the specified program file.
        /// </summary>
        /// <param name="fileName">The full path of the specified program file.</param>
        public static void AddException(string fileName)
        {
            AddRule(fileName, Path.GetFileName(fileName));
            AddRule(fileName, Path.GetFileNameWithoutExtension(fileName));
        }

        /// <summary>
        /// Removes the firewall exception of the specified program file.
        /// </summary>
        /// <param name="fileName">The full path of the specified program file.</param>
        public static void RemoveException(string fileName)
        {
            RemoveRule(fileName, Path.GetFileName(fileName));
            RemoveRule(fileName, Path.GetFileNameWithoutExtension(fileName));
        }

        /// <summary>
        /// Adds firewall exception for the specified program file through `NetFwTypeLib` COM.
        /// </summary>
        /// <param name="fileName">The full path of the specified program file.</param>
        public static void AddExceptionByCom(string fileName)
        {
            AddRuleByCom(fileName, Path.GetFileName(fileName));
            AddRuleByCom(fileName, Path.GetFileNameWithoutExtension(fileName));
        }

        /// <summary>
        /// Removes the firewall exception of the specified program file through `NetFwTypeLib` COM.
        /// </summary>
        /// <param name="fileName">The full path of the specified program file.</param>
        public static void RemoveExceptionByCom(string fileName)
        {
            RemoveRuleByCom(fileName, Path.GetFileName(fileName));
            RemoveRuleByCom(fileName, Path.GetFileNameWithoutExtension(fileName));
        }

        public static void AuthorizeApplication(string fileName)
        {
            var netFwMgr = (INetFwMgr) Activator.CreateInstance(Type.GetTypeFromProgID(FwMgr));
            var app = (INetFwAuthorizedApplication) Activator.CreateInstance(Type.GetTypeFromProgID(FwApp));

            app.Name = Path.GetFileName(fileName);
            app.ProcessImageFileName = fileName;
            app.Enabled = true;

            netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
        }

        public static void UnauthorizeApplication(string fileName)
        {
            var netFwMgr = (INetFwMgr) Activator.CreateInstance(Type.GetTypeFromProgID(FwMgr));
            netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Remove(fileName);
        }

        private static void AddRule(string appName, string ruleName)
        {
            var commandIn = $"{FirewallCmd} add rule name=\"{ruleName}\" dir=in action=allow program=\"{appName}\"";
            var commandOut = $"{FirewallCmd} add rule name=\"{ruleName}\" dir=out action=allow program=\"{appName}\"";

            CmdRunner.Execute(commandIn);
            CmdRunner.Execute(commandOut);
        }

        private static void RemoveRule(string appName, string ruleName)
        {
            var commandIn = $"{FirewallCmd} delete rule name=\"{ruleName}\" dir=in program=\"{appName}\"";
            var commandOut = $"{FirewallCmd} delete rule name=\"{ruleName}\" dir=out program=\"{appName}\"";

            CmdRunner.Execute(commandIn);
            CmdRunner.Execute(commandOut);
        }

        private static void AddRuleByCom(string appName, string ruleName)
        {
            var policy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID(FwPolicy));

            // Inbound Rule
            var ruleIn = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID(FwRule));

            ruleIn.Name = ruleName;
            ruleIn.ApplicationName = appName;
            ruleIn.Enabled = true;

            policy.Rules.Add(ruleIn);

            // Outbound Rule
            var ruleOut = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID(FwRule));

            ruleOut.Name = ruleName;
            ruleOut.ApplicationName = appName;
            ruleOut.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            ruleOut.Enabled = true;

            policy.Rules.Add(ruleOut);
        }

        private static void RemoveRuleByCom(string appName, string ruleName)
        {
            var policy = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID(FwPolicy));
            var rules = policy.Rules.OfType<INetFwRule>();

            foreach (var rule in rules.Where(x => x.Name == ruleName && x.ApplicationName == appName))
            {
                policy.Rules.Remove(rule.Name);
            }
        }

        private const string FirewallCmd = "netsh advfirewall firewall";

        private const string FwMgr = "HNetCfg.FwMgr";
        private const string FwApp = "HNetCfg.FwAuthorizedApplication";
        private const string FwPolicy = "HNetCfg.FwPolicy2";
        private const string FwRule = "HNetCfg.FWRule";
    }
}