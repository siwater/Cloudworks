/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Citrix.Cloudworks.TestClient {
    class Program {
        static void Main(string[] args) {

            TestCloudworks tester = new TestCloudworks();
            tester.RunTests();
           

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }
    }
}
