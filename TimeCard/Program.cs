using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TimeCard
{
    public class Program
    {
        // RB Added other url's to listen on
        //static readonly string[] MyUrls = {"http://localhost:5000","http://192.168.1.72:5000"};
        static readonly string[] MyUrls = {"http://0.0.0.0:5000"};
        public static void Main(string[] args)
        {
            BuildWebHost(args)
            .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(MyUrls)
                .Build();
    }
}
