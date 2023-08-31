$domainName = "compliance-services-test.fintrac-canafe.gc.ca"
$csrPath = "C:/compliance-services-test.csr"
$csrConfig = @"
 [NewRequest]
 Subject = "CN=$domainName"
 KeySpec = AT_KEYEXCHANGE
 KeyLength = 2048
 Exportable = TRUE
 RequestType = PKCS10
"@

$csrConfig | Out-File -Encoding ASCII -FilePath 'C:\csrconfig.txt'
certreq -new "C:\csrconfig.txt" $csrPath