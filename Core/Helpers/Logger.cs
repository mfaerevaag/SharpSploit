using System;
using System.Runtime.InteropServices;
using SharpSploit.Core.Exceptions;

namespace SharpSploit.Core
{
    public static class Logger
    {
        public static void Success(string message, params object[] args)
        {
            Console.Write("[+] ");
            Console.WriteLine(String.Format(message, args));
        }

        public static void Info(string message, params object[] args)
        {
            Console.Write("[*] ");
            Console.WriteLine(String.Format(message, args));
        }

        public static void Error(string message, params object[] args)
        {
            Console.Write("[-] ");
            Console.WriteLine(String.Format(message, args));
        }

        public static void Warning(string message, params object[] args)
        {
            Console.Write("[?] ");
            Console.WriteLine(String.Format(message, args));
        }

        public static void CheckError(bool check, string message, params object[] args)
        {
            if (!check)
                return;

            var userMessage = string.Format(message, args);
            var lastWinErrorMessage = string.Format("LastWinError: {0}", Marshal.GetLastWin32Error());

            var exceptionMessage = string.Format("[-] {0} ({1})", userMessage, lastWinErrorMessage);
            throw new SharpSploitException(exceptionMessage);
        }
    }
}
