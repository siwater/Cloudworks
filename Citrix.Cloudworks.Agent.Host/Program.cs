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

            CtxTrace.Initialize("stackmate-agent-host", true);
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
