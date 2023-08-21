param ($SourceURL, $TargetURL, $ClientID, $ClientSecret, $TenantID)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$targetNugetExe = ".\nuget.exe"
Remove-Item .\CLI -Force -Recurse -ErrorAction Ignore
Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
Set-Alias nuget $targetNugetExe -Scope Global -Verbose
##
##Download & Install PowerApps CLI 
##
./nuget install Microsoft.PowerApps.CLI -Version '1.25.2' -O .
Set-Location ./Microsoft.PowerApps.CLI.1.25.2
Set-Location tools
##
##Connect to source and download solution zip (to get existing solution version)
##

.\pac.exe auth create --url $SourceURL --applicationId $ClientID --tenant $TenantID --clientSecret $ClientSecret

# Logical name of the solution passed via pipeline variable
$logicalName = $env:Artifact2

# Function to determine if it's a patch or a solution based on logical name
function IsPatch {
    if ($logicalName -like '*.*.*.*') {
        return $true
    }
    else {
        return $false
    }
}

# Determine if it's a patch or a solution
$isPatch = IsPatch

# Export the solution to a zip file
$exportFileName = "$($logicalName).zip"
$exportFilePath = Join-Path -Path "." -ChildPath $exportFileName
Invoke-Expression ".\pac.exe solution export -n $logicalName --path $exportFilePath --overwrite"


# Extract solution version from the exported solution
$zipFilePath = $exportFileName
$extractPath = "./extracted_solution"
Expand-Archive -Path $zipFilePath -DestinationPath $extractPath
$manifestPath = Join-Path -Path $extractPath -ChildPath "Solution.xml"
$manifestXml = [xml](Get-Content -Path $manifestPath)
$solutionVersionString = $manifestXml.ImportExportXml.SolutionManifest.Version

# Parse the existing solution version
$existingVersion = [System.Version]::Parse($solutionVersionString)

# Increment the appropriate part based on patch or solution
$version = $existingVersion
if ($isPatch) {
    # Increment the revision part
    $version = [System.Version]::new($version.Major, $version.Minor, $version.Build, $version.Revision + 1)
}
else {
    # Increment the minor part
    $version = [System.Version]::new($version.Major, $version.Minor + 1, $version.Build, $version.Revision)
}

# Construct the new version
$newVersion = [System.Version]::new($version.Major, $version.Minor, $version.Build, $version.Revision)

# Convert the new version to a string in the format X.Y.Z.W
$solutionVersionString = $newVersion.ToString()

# Echo the solution version
Write-Host "New Solution Version: $solutionVersionString"
Invoke-Expression ".\pac.exe solution export -n $logicalName --path $exportFilePath --overwrite"

# Output the version as an output variable
Write-Host "##vso[task.setvariable variable=SolutionVersionOutput]$solutionVersionString"
# Set solution version using Power Platform CLI
Invoke-Expression ".\pac.exe solution version --strategy Solution -sp $manifestPath --buildversion $solutionVersionString"
# Delete the downloaded zip file and extracted solution
Remove-Item -Path $zipFilePath -Force
Remove-Item -Path $extractPath -Force -Recurse