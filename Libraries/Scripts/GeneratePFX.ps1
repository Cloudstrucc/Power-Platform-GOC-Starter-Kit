# Get the directory where the script is located
$scriptDirectory = $PSScriptRoot
# Specify the paths to the certificate files and where to save the PFX file
$intermediatePath = Join-Path -Path $scriptDirectory -ChildPath "../../certs/Intermediate.crt"
$rootPath = Join-Path -Path $scriptDirectory -ChildPath "../../certs/root.crt"
$serverCertificatePath = Join-Path -Path $scriptDirectory -ChildPath "../../certs/servercertificate.crt"
$pfxOutputPath = Join-Path -Path $scriptDirectory -ChildPath "../../certs/compliance-services-test.pfx"

# Load the certificates from the files
$intermediateCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($intermediatePath)
$rootCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($rootPath)
$serverCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($serverCertificatePath)

# Create a collection to store the certificates
$certificateCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$certificateCollection.Add($serverCertificate)
$certificateCollection.Add($intermediateCertificate)
$certificateCollection.Add($rootCertificate)


function GenerateRandomPassword {
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

# Specify the password for the generated PFX file
$pfxPassword = GenerateRandomPassword -Length 12
# Export the certificate collection to a PFX file
$pfxBytes = $certificateCollection.Export("Pfx", $pfxPassword)

# Save the PFX to a file
[System.IO.File]::WriteAllBytes($pfxOutputPath, $pfxBytes)

Write-Host "PFX file generated successfully at $pfxOutputPath. with password of $pfxPassword"