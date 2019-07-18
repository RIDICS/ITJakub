#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$TargetEnvironment,
	
	[Parameter(Mandatory = $true)]
	[String]$PortalType
)

$env:ASPNETCORE_ENVIRONMENT = "${TargetEnvironment}"
$DotnetVersion = & dotnet --version

$ProjectDir = $PSScriptRoot
$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using project directory: ${ProjectDir}"
Write-Host "Using .NET Core SDK ${DotnetVersion}"
Write-Host
Write-Host "Portal type: ${PortalType}"
Write-Host
Write-Host

$SelectThemeFile = Join-Path $ProjectDir "SelectPortalTheme.ps1"
$CsprojFile = Join-Path $ProjectDir "ITJakub.Web.Hub.csproj"
$PackageLocation = Join-Path $ProjectDir "bin\Publish-${TargetEnvironment}"
$OutDir = Join-Path $ProjectDir "bin\Publish-build"
$BuildPackageLocation = Join-Path $ProjectDir "bin\temp-package.zip"

$IisPath = "Default Web Site"
if ($PortalType -eq "ResearchPortal")
{
    $IisPath = "Default Web Site/Research"
}
else
{
    $IisPath = "Default Web Site/Community"
}

$GlobalSettingsFile = Join-Path $ProjectDir "globalsettings.json"
$GlobalSettingsFileBackup = Join-Path $ProjectDir "globalsettings.json.original"

# Run script for changing theme
& $SelectThemeFile $PortalType

# Restore front end dependencies and run Gulp tasks
Set-Location $ProjectDir

yarn install
if ($LASTEXITCODE -ne 0)
{
  Set-Location $CurrentPath
  exit 1
}

yarn gulp yarn-runtime
if ($LASTEXITCODE -ne 0)
{
  Set-Location $CurrentPath
  exit 1
}

# Running "gulp default" is not required because it is part of standard project build
#
# yarn gulp default
# if ($LASTEXITCODE -ne 0)
# {
#  Set-Location $CurrentPath
#  exit 1
# }

Set-Location $CurrentPath

# Create global configuration for the app
if (Test-Path $GlobalSettingsFile)
{
  Move-Item -Path $GlobalSettingsFile -Destination $GlobalSettingsFileBackup -Force
}

Set-Content -Path $GlobalSettingsFile -Value "{""EnvironmentConfiguration"": ""${TargetEnvironment}"", ""PortalType"": ""${PortalType}""}"

# Run build
dotnet build $CsprojFile --configuration Release /p:PublishProfile=Release /p:PackageLocation="${PackageLocation}" /p:OutDir="${OutDir}" /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /maxcpucount:1 /p:platform="Any CPU" /p:configuration="Release" /p:DesktopBuildPackageLocation="${BuildPackageLocation}" /p:DeployIisAppPath="${IisPath}"

if (Test-Path $GlobalSettingsFileBackup)
{
  Move-Item -Path $GlobalSettingsFileBackup -Destination $GlobalSettingsFile -Force
}
