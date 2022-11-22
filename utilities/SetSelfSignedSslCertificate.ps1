param(
    [string] $domain
) 

$cert = New-SelfSignedCertificate -DnsName @("$($domain)", "www.$($domain)") -CertStoreLocation "cert:\LocalMachine\My"
$certKeyPath = "D:\Source\TryGuessIt\Certificates\$($domain).pfx"
$password = ConvertTo-SecureString -String "pa55w0rd!" -Force -AsPlainText
$cert | Export-PfxCertificate -FilePath $certKeyPath -Password $password
