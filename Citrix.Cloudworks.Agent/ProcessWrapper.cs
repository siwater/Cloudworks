/*
 * Copyright (c) 2013-2014 Citrix Systems, Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * 'Software'), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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
