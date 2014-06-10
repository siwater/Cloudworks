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
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Citrix.Diagnostics;
using Citrix.Cloudworks.Agent;
using Citrix.Cloudworks.Agent.Services;

namespace Citrix.Cloudworks.Agent.Host {

    /// <summary>
    /// Wrapper to run the StackMate Agent as a command line console app 
    /// </summary>
    class Program {
        static void Main(string[] args) {

            CtxTrace.Initialize("cloudworks-agent-host", true);
            CloudworksServices svc = new CloudworksServices();
            new UserDataState("C:\\cfn").InitialisationComplete = false;

            svc.Start();
            Console.WriteLine("Agent is running");
            Console.WriteLine("Press any key to stop");
            Console.ReadLine();
            svc.Stop();
            Console.WriteLine("Agent is stopped");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
