using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace SharpSploit.Tests.DLLInjectionTest.Helpers
{
    class TestHelpers
    {
        public static void AssertDLLInjection(Process victim, int timeoutSeconds)
        {
            bool dllInjected = false;

            victim.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null && e.Data.Contains("DLL_INJECTED"))
                {
                    dllInjected = true;
                }
            };
            victim.BeginOutputReadLine();

            var start = DateTime.Now;
            while (!dllInjected)
            {
                // sleep introduces memory barrier
                Thread.Sleep(100);

                if (DateTime.Now.Subtract(start).TotalSeconds > timeoutSeconds)
                {
                    Assert.Fail("After waiting {0} seconds, no input was received from payload.", timeoutSeconds);
                }
            }
        }
    }
}
