Import-Module "$($PSScriptRoot)\Certificate-Module";

$_maximumDate = [DateTime]::MaxValue;
$_minimumDate = [DateTime]::MinValue.AddYears(1899);

New-RootCertificate `
	-Name "Test-root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate;

New-RootCertificate `
	-Name "To old test-root-certificate" `
	-NotAfter $_minimumDate.AddYears(1) `
	-NotBefore $_minimumDate;

New-RootCertificate `
	-Name "To young test-root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_maximumDate.AddYears(-1);

New-ClientCertificate `
	-Name "Test-client-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "Test-root-certificate";

New-ClientCertificate `
	-Name "Test-client-certificate with to old root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "To old test-root-certificate";

New-ClientCertificate `
	-Name "Test-client-certificate with to young root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "To young test-root-certificate";

New-ClientCertificate `
	-Name "To old test-client-certificate" `
	-NotAfter $_minimumDate.AddYears(1) `
	-NotBefore $_minimumDate `
	-SignerName "Test-root-certificate";

New-ClientCertificate `
	-Name "To young test-client-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_maximumDate.AddYears(-1) `
	-SignerName "Test-root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "Test-SSL-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "Test-root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "Test-SSL-certificate with to old root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "To old test-root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "Test-SSL-certificate with to young root-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate `
	-SignerName "To young test-root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "To old test-SSL-certificate" `
	-NotAfter $_minimumDate.AddYears(1) `
	-NotBefore $_minimumDate `
	-SignerName "Test-root-certificate";

New-SslCertificate `
	-DnsName "*.company.com", "localhost", "127.0.0.1" `
	-Name "To young test-SSL-certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_maximumDate.AddYears(-1) `
	-SignerName "Test-root-certificate";