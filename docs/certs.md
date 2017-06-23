# Certificates Setup (local)
`https://stackoverflow.com/questions/42351274/identityserver4-hosting-in-iis`

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



# Certificates Setup (store)

### Makecert and creating ssl or signing certificates
'https://brockallen.com/2015/06/01/makecert-and-creating-ssl-or-signing-certificates/'



# Certificates Troobleshooting

### CryptographicException 'Keyset does not exist', but only through WCF
`https://stackoverflow.com/questions/602345/cryptographicexception-keyset-does-not-exist-but-only-through-wcf/3176253#3176253`

This is most likely because the IIS user doesn't have access to the private key for your certificate. You can set this by following these steps...

1. Start -> Run -> MMC
2. File -> Add/Remove Snapin
3. Add the Certificates Snap In
4. Select Computer Account, then hit next
5. Select Local Computer (the default), then click Finish
6. On the left panel from Console Root, navigate to Certificates (Local Computer) -> Personal -> Certificates
7. Your certificate will most likely be here.
8. Right click on your certificate -> All Tasks -> Manage Private Keys
9. Set your private key settings here.

### X509Certificate - Keyset does not exist
`https://stackoverflow.com/questions/6392268/x509certificate-keyset-does-not-exist`

### Web security: threats and mitigation
`https://vimeo.com/user28557683/review/149267099/4889e181c4`

### How to: Make X.509 Certificates Accessible to WCF
`https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-make-x-509-certificates-accessible-to-wcf`

### CertificateStore's Certificates.Find() doesn't actually find the certificate
`https://stackoverflow.com/questions/14483716/certificatestores-certificates-find-doesnt-actually-find-the-certificate`

### SSL certificate for hosting Identity Server in IIS
`https://github.com/IdentityServer/IdentityServer3/issues/1684`

### X509Certificate Constructor Exception
`https://stackoverflow.com/questions/9951729/x509certificate-constructor-exception/10048789`

### X509Certificate.CreateFromCertFile - the specified network password is not correct
`https://stackoverflow.com/questions/899991/x509certificate-createfromcertfile-the-specified-network-password-is-not-corre`



