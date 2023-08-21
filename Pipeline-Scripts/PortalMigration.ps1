
param ($SourceURL, $TargetURL, $ClientID, $ClientSecret, $TenantID, $PortalSourceRootDirectory, $PortalName, $DeploymentProfile,$username, $password, $commitmessage)
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
##Download Portal 
##

$portalPath = $Env:BUILD_SOURCESDIRECTORY+'\'+$PortalSourceRootDirectory+'\'
.\pac.exe auth create --url $SourceURL --applicationId $ClientID --tenant $TenantID --clientSecret $ClientSecret
$portalQuery = .\pac.exe paportal list
$portal = '7b138792-1090-45b6-9241-8f8d96d8c372'
##.\pac.exe paportal download -id $portal -p . (o true no longer supported)
.\pac.exe paportal download -id $portal -p $portalPath -o

##
##Commit changes to repo
##

# Get the user information from environment variables
$userDisplayName = $env:BUILD_REQUESTEDFOR
$userEmail = $env:BUILD_REQUESTEDFOREMAIL
$systemAccessToken = $env:SYSTEM_ACCESSTOKEN
$comment = $env:COMMENT
# Configure Git with user information
git config --global user.email $userEmail
git config --global user.name $userDisplayName
Write-Host 'Since this is Windows Agent, set this in GIT for Windows to support longer paths than default 256'
git config --system core.longpaths true
Write-Host 'Commit all changes'               
git add .
git commit -am "$comment"
# Check if the branch exists
$branchExists = git rev-parse --verify main
if (-not $branchExists) {
    Write-Host 'Creating and checking out main branch'
    git checkout -b main
} else {
# Ensure the branch exists and is checked out
    git checkout main                  
    # Fetch and pull the latest changes from the remote main branch
    git fetch origin main
    git pull origin main
}                
Write-Host 'Push code to main branch'
git -c http.extraheader="AUTHORIZATION: bearer $systemAccessToken" push origin main
git clean -fdx

##
##Upload Portal to Target Dataverse Environment
##

## Copy downloaded portal to artefact staging directory
$portalArtefactPath = $Env:BUILD_SOURCESDIRECTORY+'\'
$deploymentProfilesPath = $portalArtefactPath+$PortalSourceRootDirectory+'\Deployment-Profiles\'
$downloadedPortalPath = $portalPath + $PortalName
Copy-Item -Path $downloadedPortalPath -Destination $portalArtefactPath -Recurse -Force
Copy-Item -Path $deploymentProfilesPath -Destination $downloadedPortalPath -Recurse -Force

.\pac.exe auth create --url $TargetURL --applicationId $ClientID --tenant $TenantID --clientSecret $ClientSecret
$portalQuery = .\pac.exe paportal list
$portal = $portalQuery[5].substring(12,36)
.\pac.exe paportal upload -p $downloadedPortalPath --deploymentProfile $DeploymentProfile

Set-Location ..\..\
Remove-Item Microsoft.PowerApps.CLI.1.25.2 -Force -Recurse -ErrorAction Ignore
Remove-Item nuget.exe -Recurse