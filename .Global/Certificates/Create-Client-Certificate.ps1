Import-Module "$($PSScriptRoot)\Certificate-Module";

New-ClientCertificate `
	-Name "Certificate Lab - Client";