using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using Labs.Security.Auth.Quickstart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Labs.Security.Auth
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            Environment = environment;
            
            var loggerPath = Path.Combine(@"c:\Data\logs\security", $"security.auth.{Environment.EnvironmentName.ToLower()}.logs.txt");

            var loggerOptions = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(loggerPath);

            if (Environment.IsDevelopment())
            {
                loggerOptions.WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}");
            }

            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "IdentityServer4", LogLevel.Debug },
                    { "Microsoft", LogLevel.Warning },
                    { "System", LogLevel.Warning },
                })
                .AddSerilog(loggerOptions.CreateLogger());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ToDo: Consider serving certificate from Trusted Store
            var certificatePath = Path.Combine(Environment.ContentRootPath, "Certificates", "IdentityServerAuth.pfx");
            var certificate = new X509Certificate2(certificatePath);

            services.AddMvc();

            services
                .AddIdentityServer()
                //.AddDeveloperSigningCredential()
                .AddSigningCredential(certificate)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(TestUsers.Users);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer();

            // middleware for google authentication
            // must use http://localhost:5000 for this configuration to work
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
                ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}