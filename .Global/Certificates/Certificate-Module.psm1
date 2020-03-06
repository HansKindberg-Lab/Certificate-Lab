$_certificateStoreLocation = "CERT:\CurrentUser\My\";
$_hashAlgorithm = "sha256";
$_keyExportPolicy = "Exportable";
$_keyLength = 2048;
$_keySpecification = "Signature";
$_password = ConvertTo-SecureString -AsPlainText -Force -String "password";

function New-Certificate
{
	param
	(
		[string[]]$dnsName,
		$keyUsage,
		$keyUsageProperty,
		[Parameter(Mandatory)]
		[string]$name,
		[DateTime]$notAfter = (Get-Date).AddYears(1),
		[DateTime]$notBefore = (Get-Date).AddMinutes(-10),
		[string]$signerName,
		[string[]]$textExtension,
		$type
	)

	$parameters = @{
		CertStoreLocation = $_certificateStoreLocation
		FriendlyName = $name
		HashAlgorithm = $_hashAlgorithm
		KeyExportPolicy = $_keyExportPolicy
		KeyLength = $_keyLength
		KeySpec = $_keySpecification
		NotAfter = $notAfter
		NotBefore = $notBefore
		Subject = "CN=$($name)"
		TextExtension = $textExtension
	};

	if($dnsName)
	{
		$parameters.Add("DnsName", $dnsName);
	}

	if($keyUsage)
	{
		$parameters.Add("KeyUsage", $keyUsage);
	}

	if($keyUsageProperty)
	{
		$parameters.Add("KeyUsageProperty", $keyUsageProperty);
	}

	if($signerName)
	{
		$signer = Import-PfxCertificate -CertStoreLocation $_certificateStoreLocation -FilePath "$($PSScriptRoot)\$($signerName).pfx" -Password $_password;

		$parameters.Add("Signer", $signer);
	}

	if($type)
	{
		$parameters.Add("Type", $type);
	}

	$certificate = New-SelfSignedCertificate @parameters;

	Export-PfxCertificate -Cert $certificate -FilePath "$($PSScriptRoot)\$($name).pfx" -Password $_password;

	if($signer)
	{
		Remove-Item -Path "$($_certificateStoreLocation)$($signer.Thumbprint)";
	}
	else
	{
		Export-Certificate -Cert $certificate -FilePath "$($PSScriptRoot)\$($name).cer";
	}

	Remove-Item -Path "$($_certificateStoreLocation)$($certificate.Thumbprint)";
}

function New-ClientCertificate
{
	param
	(
		[Parameter(Mandatory)]
		[string]$name,
		[DateTime]$notAfter = (Get-Date).AddYears(1),
		[DateTime]$notBefore = (Get-Date).AddMinutes(-10),
		[string]$signerName
	)

	New-Certificate `
		-Name $name `
		-NotAfter $notAfter `
		-NotBefore $notBefore `
		-SignerName $signerName `
		-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2");
}

function New-RootCertificate
{
	param
	(
		[Parameter(Mandatory)]
		[string]$name,
		[DateTime]$notAfter = (Get-Date).AddYears(1),
		[DateTime]$notBefore = (Get-Date).AddMinutes(-10)
	)

	New-Certificate `
		-KeyUsage "CertSign" `
		-KeyUsageProperty "Sign" `
		-Name $name `
		-NotAfter $notAfter `
		-NotBefore $notBefore `
		-Type "Custom";
}

function New-SslCertificate
{
	param
	(
		[Parameter(Mandatory)]
		[string[]]$dnsName,
		[Parameter(Mandatory)]
		[string]$name,
		[DateTime]$notAfter = (Get-Date).AddYears(1),
		[DateTime]$notBefore = (Get-Date).AddMinutes(-10),
		[string]$signerName
	)

	New-Certificate `
		-DnsName $dnsName `
		-Name $name `
		-NotAfter $notAfter `
		-NotBefore $notBefore `
		-SignerName $signerName;
}

Export-ModuleMember -Function "New-ClientCertificate", "New-RootCertificate", "New-SslCertificate";