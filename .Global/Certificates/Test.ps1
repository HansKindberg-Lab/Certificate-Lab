Import-Module "$($PSScriptRoot)\Certificate-Module";

New-RootCertificate `
	-Name "Root-certificate";

New-ClientCertificate `
	-Name "Client-certificate" `
	-SignerName "Root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "SSL-certificate" `
	-SignerName "Root-certificate";