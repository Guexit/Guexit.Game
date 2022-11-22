```powershell
$cert = New-SelfSignedCertificate -DnsName @("local-tryguessit.com", "www.local-tryguessit.com") -CertStoreLocation "cert:\LocalMachine\My"
$certKeyPath = "D:\Source\TryGuessIt\Certificates\tryguessit.com.pfx"
$password = ConvertTo-SecureString -String "pa55w0rd!" -Force -AsPlainText
$cert | Export-PfxCertificate -FilePath $certKeyPath -Password $password
```