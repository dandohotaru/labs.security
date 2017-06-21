# Framework Setup

### Use ASP.NET Core 1.1 with .NET 4.6 (without Visual Studio)
`https://jonhilton.net/2017/03/14/use-asp-net-core-1-1-with-net-4-without-visual-studio/`

### How to change target framework with VS 2017 RC?
`https://stackoverflow.com/questions/41433548/how-to-change-target-framework-with-vs-2017-rc`



# Certificates Setup ([hint](https://stackoverflow.com/questions/42351274/identityserver4-hosting-in-iis))

### Create the certificate in the project
```
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\makecert" -n "CN=IdentityServerAuth" -a sha256 -sv IdentityServerAuth.pvk -r IdentityServerAuth.cer -b 01/01/2017 -e 01/01/2025
```

```
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\pvk2pfx" -pvk IdentityServerAuth.pvk -spc IdentityServerAuth.cer -pfx IdentityServerAuth.pfx
```

### Make sure "AddSigningCredential" is being used
```
services
	.AddIdentityServer()
    .AddSigningCredential(new X509Certificate2(Path.Combine(Environment.ContentRootPath, "certificates", "IdentityServerAuth.pfx")))
	...
```

### Test with iis express

### If successful, deploy to iis server, install the certificate on the server by double clicking on it, and test

### Make sure the application pool "load user profile" is set to true

- Go to IIS Manager
- Go to the application pool instance
- Click advanced settings
- Under Process model, set Load User Profile to true

### Restart IIS



# Deployment Strategies

### Create publish profiles for Visual Studio and MSBuild, to deploy ASP.NET Core apps
`https://docs.microsoft.com/en-us/aspnet/core/publishing/web-publishing-vs`


# Deployment Configuration

### Setting environment variables for asp.net core when publishing on IIS
`http://www.andrecarlucci.com/en/setting-environment-variables-for-asp-net-core-when-publishing-on-iis/`

### How to set the hosting environment in ASP.NET Core
`https://andrewlock.net/how-to-set-the-hosting-environment-in-asp-net-core/`

### Setting Asp.Net Core Environment Variables via web.config in IIS
`https://blog.dangl.me/archive/setting-asp-net-core-environment-variables-via-web-config-in-iis/`

### Make sure feature delegation is enabled for authentication
`https://serverfault.com/questions/529819/changes-done-in-iis-manager-are-reflected-in-web-config-intead-of-applicationhos`


# Deployment Troubleshooting

### Web Deploy error codes
`https://docs.microsoft.com/en-us/iis/publish/troubleshooting-web-deploy/web-deploy-error-codes#ERROR_FILE_IN_USE`

### Taking an Application Offline before Publishing
`https://docs.microsoft.com/en-us/iis/publish/deploying-application-packages/taking-an-application-offline-before-publishing`

### Web publishing updates for app offline and usechecksum
`https://blogs.msdn.microsoft.com/webdev/2013/10/29/web-publishing-updates-for-app-offline-and-usechecksum/`

