using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSploit.Core;
using SharpSploit.DLLInjection;
using SharpSploit.Tests.DLLInjectionTest.Helpers;

namespace SharpSploit.Tests.DLLInjectionTest
{
    [TestClass]
    public class InjectionTest
    {
        private Process _victim;
        private Payload _payload;
        private FileInfo _payloadDll;

        private const int TIMEOUT = 3;

        [TestInitialize]
        public void SetUp()
        {
            // generate payload
            _payload = Payload.Generate(@"
                char message[] = ""DLL_INJECTED\r\n"";
                DWORD tmp;
                WriteFile(hStdout, message, sizeof(message), &tmp, NULL);
            ");

            // compile payload to dll
            _payloadDll = _payload.Compile();

            // start victim process
            _victim = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = false,
                FileName = typeof(DummyVictim.Program).Assembly.Location,

                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            });
        }

        [TestCleanup]
        public void TearDown()
        {
            // kill victim process
            try
            {
                _victim.StandardInput.WriteLine("quit");

                if (!_victim.WaitForExit((int)TimeSpan.FromSeconds(TIMEOUT).TotalMilliseconds))
                {
                    Logger.Warning("Victim process didn't quit as usual. Killing...");
                    _victim.Kill();
                }
            }
            catch
            {
                _victim.Kill();
            }
            finally
            {
                _victim.Dispose();
            }

            // delete dll
            _payloadDll.Delete();
        }

        [TestMethod]
        public void Should_Inject_DLL_Using_Create_Remote_Thread_Method()
        {
            Injection.Factory(Injection.Method.CREATE_REMOTE_THREAD).Inject(_victim.Id, _payloadDll);

            TestHelpers.AssertDLLInjection(_victim, timeoutSeconds: TIMEOUT);
        }
    }
}
