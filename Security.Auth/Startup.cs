﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using Labs.Security.Auth.Data;
using Labs.Security.Auth.Quickstart.Account;
using Labs.Security.Domain.Adfs.Profiles;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Features.Users;
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
            var certificate = new Func<string, X509Certificate2>(name =>
            {
                using (var store = new X509Store(StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.ReadOnly);
                    var found = store.Certificates.Find(X509FindType.FindBySubjectName, name, validOnly: false);
                    store.Close();
                    return found.Count > 0
                        ? found[index: 0]
                        : null;
                }
            });

            // Add mvc
            services.AddMvc();

            // Add extras
            services.AddTransient<IIdentityProvider, DirectoryIdentityProvider>();
            services.AddSingleton<IUserStore, UserStore>(context =>
            {
                var provider = context.GetService<IIdentityProvider>();
                return new LocalStore(provider);
            });

            services.Configure<IISOptions>(options =>
            {
                options.ForwardWindowsAuthentication = true;
            });

            // Add idsrv
            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddSigningCredential(certificate("sec.auth"))
                .AddInMemoryIdentityResources(new CarbonData().Load())
                .AddInMemoryApiResources(new SiliconData().Load())
                .AddInMemoryClients(new ClientsData().Load())
                .AddProfileService<ProfileService>();
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
                    ClientId = "15987029996-5l1d4bmgo0b3qa3da7pt9gmr2jrs8los.apps.googleusercontent.com",
                    ClientSecret = "19TvpEYaPMNSVHWi6GrktSPC"
                });

                inner.UseMvcWithDefaultRoute();
                inner.UseStaticFiles();
            });
        }
    }
}