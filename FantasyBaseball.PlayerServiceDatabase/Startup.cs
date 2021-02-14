using FantasyBaseball.PlayerServiceDatabase.Database;
using FantasyBaseball.PlayerServiceDatabase.Services;
using Mcrio.Configuration.Provider.Docker.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FantasyBaseball.PlayerServiceDatabase
{
    /// <summary>The class that sets up all of the configuration for the service.</summary>
    public class Startup
    {
        /// <summary>Creates a new instance of the startup and sets up the configuration object.</summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public Startup(IHostEnvironment env) =>
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddDockerSecrets()
                .Build();

        /// <summary>Represents a set of key/value application configuration properties.</summary>
        public IConfiguration Configuration { get; }

        /// <summary>This method configures the HTTP request pipeline.</summary>
        /// <param name="app">The object to convert to a string.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env) 
        {
            app
                .UseCors()
                .UseHsts()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseEndpoints(endpoints => endpoints.MapHealthChecks("/api/health"));
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<PlayerContext>().Database.Migrate();
        }

        /// <summary>This method adds the services to the container.</summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = string.Format(Configuration.GetConnectionString("PlayerDatabase"), 
                Configuration["player-database-user"], Configuration["player-database-password"]);
            services.AddHealthChecks().AddDbContextCheck<PlayerContext>();
            services
                .AddCors(options => options.AddDefaultPolicy(builder => builder
                    .WithOrigins("http://*.schultz.local", "http://localhost")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()))
                .AddSingleton(Configuration)
                .AddDbContext<PlayerContext>(options => options.UseNpgsql(connectionString))
                .AddScoped<IPlayerContext>(provider => provider.GetService<PlayerContext>())
                .AddSingleton<IBaseballPlayerBuilderService, BaseballPlayerBuilderService>()
                .AddScoped<IGetPlayersService, GetPlayersService>()
                .AddSingleton<IPlayerEntityMergerService, PlayerEntityMergerService>()
                .AddScoped<IPlayerUpdateService, PlayerUpdateService>()
                .AddScoped<IUpsertPlayersService, UpsertPlayersService>()
                .AddControllers();
        }
    }
}