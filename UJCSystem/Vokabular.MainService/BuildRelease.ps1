#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$TargetEnvironment
)

$env:ASPNETCORE_ENVIRONMENT = "${TargetEnvironment}"
$DotnetVersion = & dotnet --version

$ProjectDir = $PSScriptRoot

Write-Host
Write-Host "Using project directory: ${ProjectDir}"
Write-Host "Using .NET Core SDK ${DotnetVersion}"
Write-Host
Write-Host

$CsprojFile = Join-Path $ProjectDir "Vokabular.MainService.csproj"
$PackageLocation = Join-Path $ProjectDir "bin\Publish-${TargetEnvironment}"
$OutDir = Join-Path $ProjectDir "bin\Publish-build"
$BuildPackageLocation = Join-Path $ProjectDir "bin\temp-package.zip"

$IisPath = "Default Web Site/MainService"

$GlobalSettingsFile = Join-Path $ProjectDir "globalsettings.json"
$GlobalSettingsFileBackup = Join-Path $ProjectDir "globalsettings.json.original"


if (Test-Path $GlobalSettingsFile)
{
  Move-Item -Path $GlobalSettingsFile -Destination $GlobalSettingsFileBackup -Force
}

Set-Content -Path $GlobalSettingsFile -Value "{""EnvironmentConfiguration"": ""${TargetEnvironment}""}"

dotnet build $CsprojFile --configuration Release /p:PublishProfile=Release /p:PackageLocation="${PackageLocation}" /p:OutDir="${OutDir}" /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /maxcpucount:1 /p:platform="Any CPU" /p:configuration="Release" /p:DesktopBuildPackageLocation="${BuildPackageLocation}" /p:DeployIisAppPath="${IisPath}"

if (Test-Path $GlobalSettingsFileBackup)
{
  Move-Item -Path $GlobalSettingsFileBackup -Destination $GlobalSettingsFile -Force
}
