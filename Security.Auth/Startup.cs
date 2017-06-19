using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using Labs.Security.Auth.Quickstart;
using Labs.Security.Auth.Quickstart.Account;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Labs.Security.Auth
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            Environment = environment;

            var loggerPath = Path.Combine(@"c:\Data\logs\security", $"security.auth.{Environment.EnvironmentName.ToLower()}.logs.txt");

            var loggerOptions = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File(loggerPath)
                .WriteTo.Seq("http://localhost:5341");

            if (Environment.IsDevelopment())
            {
                loggerOptions.WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}");
            }

            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    {"IdentityServer4", LogLevel.Debug},
                    {"Microsoft", LogLevel.Warning},
                    {"System", LogLevel.Warning},
                })
                .AddSerilog(loggerOptions.CreateLogger());
        }

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // ToDo: Consider serving certificate from a Trusted Store [DanD]
            var certificatePath = Path.Combine(Environment.ContentRootPath, "Certificates", "IdentityServerAuth.pfx");
            var certificate = new X509Certificate2(certificatePath);

            services.AddMvc();
            
            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    
                })
                .AddSigningCredential(certificate)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddProfileService<ProfileService>()
                .AddAuthorizeInteractionResponseGenerator<AuthorizeInteractionResponseGenerator>();
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

            app.Map("/core", inner =>
            {
                inner.UseIdentityServer();

                // Expects http://localhost:5000 for this configuration to work
                inner.UseGoogleAuthentication(new GoogleOptions
                {
                    AuthenticationScheme = "Google",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
                    ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
                });

                inner.UseMvcWithDefaultRoute();
                inner.UseStaticFiles();
            });
        }
    }
}