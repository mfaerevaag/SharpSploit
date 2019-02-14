using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpSploit.Core;
using SharpSploit.DLLInjection;

namespace SharpSploit.Console
{
    public class Program
    {
        const string dllPath = "C:\\Users\\mfaerevaag\\Dev\\GameHacking-DLLInjection-Unfinished\\Dll_test.dll";

        const string procName = "ac_client";

        static void Main(string[] args)
        {
            //// check dll file
            //if (!File.Exists(dllPath))
            //{
            //    throw new ArgumentException(string.Format("Cannot access DLL: '{0}'", dllPath));
            //}

            //// get process
            //Process[] procList = Process.GetProcessesByName(procName);
            //if (procList.Length == 0)
            //{
            //    Logger.Error("No process '{0}' found", procName);
            //    throw new ArgumentException();
            //}
            //else if (procList.Length > 1)
            //{
            //    Logger.Error("Multiple '{0} processes found", procName);
            //    throw new ArgumentException();
            //}
            //Process proc = procList[0];

            //FileInfo dll = new FileInfo(dllPath);

            //// go
            //Injection.LoadLib(proc.Id, dll);


            // generate payload
            Payload payload = Payload.Generate(@"
                MessageBox(NULL, _T(""Your process belongs to me""), _T(""SharpSploit""), 0);
                break;
            ");

            // compile payload to dll
            FileInfo payloadDll = null;
            try
            {
                 payloadDll = payload.Compile();
            }
            catch (PayloadException ex)
            {
                Logger.Error(ex.Message);
                return;
            }

            // start victim process
            Process victim = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = false,
                FileName = typeof(Tests.DummyVictim.Program).Assembly.Location,

                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            });


            Injection.Factory(Injection.Method.CREATE_REMOTE_THREAD).Inject(victim.Id, payloadDll);

            // wait
            var start = DateTime.Now;
            while (true)
            {
                Thread.Sleep(100);
                if (DateTime.Now.Subtract(start).TotalSeconds > 10)
                    break;
            }

            victim.Kill();
            victim.Dispose();
            payloadDll.Delete();
        }
    }
}
