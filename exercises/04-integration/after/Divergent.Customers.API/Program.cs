﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace Divergent.Customers.API
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            ConsoleEx.TryMinimize();

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { tcs.SetResult(null); };

            using (WebApp.Start<Startup>("http://localhost:20186"))
            {
                await Console.Out.WriteLineAsync("Web server is running.");
                await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

                await tcs.Task;
            }
        }
    }
}
