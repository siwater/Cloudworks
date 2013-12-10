using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {
    public class ProcessWrapper {

        private ProcessStartInfo startInfo;

        private StreamWriter Out;
        private StreamWriter Err;

        public ProcessWrapper(string executable, string args, string workingDirectory) {
            startInfo = new ProcessStartInfo() {
                FileName = executable,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };
            Out = new StreamWriter(Path.Combine(workingDirectory, "stdout.log"));
            Err = new StreamWriter(Path.Combine(workingDirectory, "stderr.log"));
        }

        public void Execute() {
            CtxTrace.TraceInformation("Executable = {0}", startInfo.FileName);
            try {
                using (var process = new Process()) {
                    process.StartInfo = this.startInfo;
                    process.OutputDataReceived += process_OutputDataReceived;
                    process.ErrorDataReceived += process_ErrorDataReceived;
                    process.Start();
                    process.BeginOutputReadLine();             
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                    CtxTrace.TraceInformation("Child process has exited");
                }
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            } finally {
                Out.Close();
                Err.Close();
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                Out.WriteLine(e.Data);
                Out.Flush();
            } catch (Exception ex) {
                CtxTrace.TraceError(ex);
            }
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            try {
                Err.WriteLine(e.Data);
                Err.Flush();
            } catch (Exception ex) {
                CtxTrace.TraceError(ex);
            }
        }
    }
}
