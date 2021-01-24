using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FantasyBaseball.PlayerServiceDatabase
{
    /// <summary>The class that kicks off everything off.</summary>
    public static class Program
    {
        /// <summary>The starting point of the application.</summary>
        /// <param name="args">No args are used.</param>
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();
        
        private static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}