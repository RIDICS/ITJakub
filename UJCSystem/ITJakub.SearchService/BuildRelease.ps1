#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$TargetEnvironment
)

$env:ASPNETCORE_ENVIRONMENT = "${TargetEnvironment}"
$MsBuildPath = & "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

$ProjectDir = $PSScriptRoot

Write-Host
Write-Host "Using project directory: ${ProjectDir}"
Write-Host "Using MSBuild path: ${MsBuildPath}"
Write-Host
Write-Host

$WebConfigTransformFileTarget = Join-Path $ProjectDir "Web.${TargetEnvironment}.config"
$WebConfigTransformFileSource = "C:\Pool\itjakub-secrets\WcfServices\Web.${TargetEnvironment}.config"
$CsprojFile = Join-Path $ProjectDir "ITJakub.SearchService.csproj"
$PackageLocation = Join-Path $ProjectDir "bin\Publish-${TargetEnvironment}"
$OutDir = Join-Path $ProjectDir "bin\Publish-build"
$BuildPackageLocation = Join-Path $ProjectDir "bin\temp-package.zip"

$IisPath = "LocalhostServices/ITJakub.SearchService"

if (Test-Path $WebConfigTransformFileSource)
{
    Copy-Item $WebConfigTransformFileSource -Destination $WebConfigTransformFileTarget
}

& $MsBuildPath $CsprojFile /p:PublishProfile=Package /p:EnvironmentName=${TargetEnvironment} /p:PackageLocation="${PackageLocation}" /p:OutDir="${OutDir}" /p:OutputPath="${OutDir}" /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /maxcpucount:1 /p:platform="Any CPU" /p:configuration="${TargetEnvironment}" /p:DesktopBuildPackageLocation="${BuildPackageLocation}" /p:DeployIisAppPath="${IisPath}" -verbosity:minimal

if (Test-Path $WebConfigTransformFileSource)
{
    Remove-Item $WebConfigTransformFileTarget -Force
}
