# Specify the paths to the certificate files and where to save the PFX file
$intermediatePath = "..\certs\Intermediate.crt"
$rootPath = "..\certs\root.crt"
$serverCertificatePath = "..\certs\servercertificate.crt"
$pfxOutputPath = "..\certs\compliance-services-test.pfx"

# Specify the password for the generated PFX file
$pfxPassword = Generate-RandomPassword -Length 16

# Load the certificates from the files
$intermediateCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($intermediatePath)
$rootCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($rootPath)
$serverCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($serverCertificatePath)

# Create a collection to store the certificates
$certificateCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$certificateCollection.Add($serverCertificate)
$certificateCollection.Add($intermediateCertificate)
$certificateCollection.Add($rootCertificate)

# Export the certificate collection to a PFX file
$pfxBytes = $certificateCollection.Export("Pfx", $pfxPassword)

# Save the PFX to a file
[System.IO.File]::WriteAllBytes($pfxOutputPath, $pfxBytes)

function Generate-RandomPassword {
    param(
        [int]$Length = 12
    )
    
    $ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:',.<>?`~"
    $Password = ""
    
    for ($i = 0; $i -lt $Length; $i++) {
        $RandomIndex = Get-Random -Minimum 0 -Maximum $ValidCharacters.Length
        $Password += $ValidCharacters[$RandomIndex]
    }
    
    return $Password
}

Write-Host "PFX file generated successfully at $pfxOutputPath. with password of $pfxPassword"