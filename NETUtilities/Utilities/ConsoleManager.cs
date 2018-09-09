using System;
using System.Runtime.InteropServices;

namespace Utilities
{
    /// <summary>
    /// Manage the attached console for calling process.
    /// </summary>
    public static class ConsoleManager
    {
        private static bool _hasConsole;
        private static bool _hasCreated;

        /// <summary>
        /// Show the attached console.
        /// </summary>
        public static void Show()
        {
            // Already has attached console?
            if (GetConsoleWindow() != IntPtr.Zero)
            {
                _hasConsole = true;
                return;
            }

            // Try to attach the console of parent process
            if (AttachConsole(-1))
            {
                _hasConsole = true;
                return;
            }

            // To alloc a new console
            _hasCreated = AllocConsole();
            _hasConsole = _hasCreated;
        }

        /// <summary>
        /// Close the attached console.
        /// </summary>
        public static void Close()
        {
            if (_hasCreated)
            {
                FreeConsole();
            }

            _hasCreated = false;
            _hasConsole = false;
        }

        /// <summary>
        /// Writes the message, followed by the current line terminator, to the attached console.
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message)
        {
            EnsureConsole();
            Console.WriteLine(message);
        }

        /// <summary>
        /// Writes the message to the attached console.
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            EnsureConsole();
            Console.Write(message);
        }

        /// <summary>
        /// Make sure the attached console exists.
        /// </summary>
        private static void EnsureConsole()
        {
            if (!_hasConsole)
            {
                Show();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

    }
}