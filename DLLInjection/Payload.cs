using SharpSploit.Core;
using System;
using System.Diagnostics;
using System.IO;

namespace SharpSploit.DLLInjection
{
    public class Payload
    {
        public string Content { get; set; }

        public const string SKELETON_PLACEHOLDER_BODY = "REPLACE_BODY";
        public const string SKELETON_PLACEHOLDER_SLEEP = "REPLACE_SLEEP";
        public const int DEFAULT_SLEEP = 1;

        public Payload(string content) => Content = content;

        public static Payload Generate(string body, int sleepSeconds = DEFAULT_SLEEP)
        {
            string content = Properties.Resources.PayloadSkeleton
                .Replace(SKELETON_PLACEHOLDER_BODY, body)
                .Replace(SKELETON_PLACEHOLDER_SLEEP, ((int)TimeSpan.FromSeconds(sleepSeconds).TotalMilliseconds).ToString());
            return new Payload(content);
        }

        public FileInfo Compile()
        {
            // work in temp
            string pathDir = Path.GetTempPath();

            // Logger.Info("Compiling payload:\n {0}", this.Content);

            // write payload to temp file
            string tempName = Guid.NewGuid().ToString() + ".cpp";
            string tempPath = pathDir + tempName;
            string outputDll = tempPath.Replace(".cpp", ".dll");

            Logger.Info("Writing payload to {0}...", tempPath);
            File.WriteAllText(tempPath, this.Content);

            Logger.Info("Compiling...");

            // setup process
            Process compiler = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WorkingDirectory = pathDir,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            });

            // run vcvars to setup env first
            // TODO: Find VS version automatically
            // https://github.com/xen2/SharpLang/blob/07902915970ace70c4ee0430a672d25187a75d3a/src/SharpLang.Compiler/Toolchains/MSVCToolchain.cs#L122-L158
            //compiler.StandardInput.WriteLine("\"" + @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\vcvars32.bat" + "\"");
            compiler.StandardInput.WriteLine("\"" + @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars32.bat" + "\"");
            compiler.StandardInput.WriteLine(@"cl.exe /LD " + tempName);
            compiler.StandardInput.WriteLine(@"exit");

            // get output
            string output = compiler.StandardOutput.ReadToEnd();
            compiler.WaitForExit();
            Logger.Info(output);

            // delete temp file
            File.Delete(tempPath);
            int exitCode = compiler.ExitCode;
            compiler.Close();

            // check compiler exit code
            if (exitCode != 0)
            {
                Logger.Error("Compiler exited with code {0}", exitCode);
                throw new PayloadException("Failed to compile payload");
            }

            Logger.Success("Payload compiled successfully");

            // return dll
            return new FileInfo(outputDll);
        }
    }
}
