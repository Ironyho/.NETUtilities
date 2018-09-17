using System.IO;

namespace Utilities
{
    /// <summary>
    /// Provide methods to add\remove\check filewall exception.
    /// </summary>
    public class FirewallOperator
    {
        /// <summary>
        /// Determines whether filewall exception exists.
        /// </summary>
        /// <param name="fileName"></param>
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
        /// Adds filewall exception for a file.
        /// </summary>
        /// <param name="fileName"></param>
        public static void AddException(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var nameWithEx = fileInfo.Name;

            // name with extension
            var commandIn = $"{FirewallCmd} add rule name=\"{nameWithEx}\" dir=in action=allow program=\"{fileName}\"";
            var commandOut =
                $"{FirewallCmd} add rule name=\"{nameWithEx}\" dir=out action=allow program=\"{fileName}\"";

            CmdRunner.Execute(commandIn);
            CmdRunner.Execute(commandOut);

            // name without extension
            var name = Path.GetFileNameWithoutExtension(fileName);
            if (!string.IsNullOrEmpty(name))
            {
                commandIn = $"{FirewallCmd} add rule name=\"{name}\" dir=in action=allow program=\"{fileName}\"";
                commandOut = $"{FirewallCmd} add rule name=\"{name}\" dir=out action=allow program=\"{fileName}\"";

                CmdRunner.Execute(commandIn);
                CmdRunner.Execute(commandOut);
            }
        }

        /// <summary>
        /// Removes filrewall exception for a file.
        /// </summary>
        /// <param name="fileName"></param>
        public static void RemoveException(string fileName)
        {
            // name with extension
            var nameWithEx = new FileInfo(fileName).Name;

            var commandIn = $"{FirewallCmd} delete rule name=\"{nameWithEx}\" dir=in program=\"{fileName}\"";
            var commandOut = $"{FirewallCmd} delete rule name=\"{nameWithEx}\" dir=out program=\"{fileName}\"";

            CmdRunner.Execute(commandIn);
            CmdRunner.Execute(commandOut);

            // name without extension
            var name = Path.GetFileNameWithoutExtension(fileName);
            if (!string.IsNullOrEmpty(fileName))
            {
                commandIn = $"{FirewallCmd} delete rule name=\"{name}\" dir=in program=\"{fileName}\"";
                commandOut = $"{FirewallCmd} delete rule name=\"{name}\" dir=out program=\"{fileName}\"";

                CmdRunner.Execute(commandIn);
                CmdRunner.Execute(commandOut);
            }
        }

        private static string FirewallCmd { get; } = "netsh advfirewall firewall";
    }
}