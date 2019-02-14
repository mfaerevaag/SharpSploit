using System;
using System.Diagnostics;

namespace SharpSploit.Tests.DummyVictim
{
    public class Program
    {
        static void Main(string[] args)
        {
            int pid = Process.GetCurrentProcess().Id;
            Console.WriteLine("PID: {0}", pid);

            string userInput;
            while ((userInput = Console.ReadLine()) != "quit")
                ;
        }
    }
}
