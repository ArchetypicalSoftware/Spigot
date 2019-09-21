﻿using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Spigot.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseElectron(args);
    }
}