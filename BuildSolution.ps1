#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$TargetEnvironment,

    [Parameter(Mandatory = $false)]
    [Boolean]$CleanBuildDir = $true
)

$env:ASPNETCORE_ENVIRONMENT = "${TargetEnvironment}"

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host


$buildDir = Join-Path $CurrentPath -ChildPath "build"
$SolutionDir = Join-Path $CurrentPath -ChildPath "UJCSystem"

New-Item -Force -Path $buildDir -Name "Publish-${TargetEnvironment}" -ItemType "directory" > $null

$OutputDir = Join-Path $buildDir "Publish-${TargetEnvironment}"

Write-Host "Using output directory: ${OutputDir}"

if ($CleanBuildDir)
{
    $OutputDirRm = Join-Path $buildDir "Publish-${TargetEnvironment}.rm"

    Move-Item $OutputDir $OutputDirRm -Force
    Remove-Item $OutputDirRm -Recurse -Force

    New-Item -Path $buildDir -Name "Publish-${TargetEnvironment}" -ItemType "directory" > $null
}

# $MigratorProjectBuild = Join-Path $SolutionDir "Ridics.Authentication.Database.Migrator\BuildRelease.cmd"

$ServiceProjectsToBuild = @()
$ServiceProjectsToBuild += "ITJakub.FileProcessing.Service"
$ServiceProjectsToBuild += "ITJakub.Lemmatization.Service"
$ServiceProjectsToBuild += "ITJakub.SearchService"
$ServiceProjectsToBuild += "ITJakub.Web.Hub"
$ServiceProjectsToBuild += "Vokabular.FulltextService"
$ServiceProjectsToBuild += "Vokabular.MainService"

# Write-Host "${MigratorProjectBuild}"
foreach ($ProjectToBuild in $ServiceProjectsToBuild)
{
  Write-Host $ProjectToBuild
}

Write-Host
Write-Host


function BuildServiceProject {
    Param ([String]$ProjectName)

    Write-Host
    Write-Host

	$BuildScriptPath = Join-Path $SolutionDir "${ProjectName}\BuildRelease.ps1"

    & $BuildScriptPath ${TargetEnvironment}

    if ($LASTEXITCODE -eq 0)
    {
        $ProjectPublish = Join-Path $SolutionDir "${ProjectName}\bin\Publish-${TargetEnvironment}\*"

        Copy-Item $ProjectPublish -Destination $OutputDir -Recurse

        if ($LASTEXITCODE -ne 0)
        {
            Write-Error "Copy ${ProjectName} failed"
            exit 1
        }
    }
    else
    {
        Write-Error "Build ${ProjectName} failed"
        exit 1
    }
}

# Migrator is currently not used in this solution
function BuildMigrator {

    Write-Host
    Write-Host

    & cmd /c "${MigratorProjectBuild} ${TargetEnvironment}"

    if ($LASTEXITCODE -eq 0)
    {
        # Migrator use ASPNETCORE_ENVIRONMENT variable for filtering files during build internally in CSPROJ file
        $MigratorProjectPublish = Join-Path $SolutionDir "Ridics.Authentication.Database.Migrator\bin\Migrator-build\*"

        New-Item -Force -Path $OutputDir -Name "Ridics.Authentication.Database.Migrator" -ItemType "directory" > $null

        $MigratorOutputDir = Join-Path $OutputDir "Ridics.Authentication.Database.Migrator"

        $MigratorOutputDirRm = Join-Path $buildDir "Ridics.Authentication.Database.Migrator.rm"

        Move-Item $MigratorOutputDir $MigratorOutputDirRm -Force
        Remove-Item $MigratorOutputDirRm -Recurse -Force

        New-Item -Force -Path $OutputDir -Name "Ridics.Authentication.Database.Migrator" -ItemType "directory" > $null

        Copy-Item $MigratorProjectPublish -Destination $MigratorOutputDir -Recurse
        #Move-Item (Join-Path $MigratorOutputDir "Migrate.${TargetEnvironment}.ps1") -Destination $OutputDir

        if ($LASTEXITCODE -ne 0)
        {
            Write-Error "Copy migrator failed"
            exit 1
        }
    }
    else
    {
        Write-Error "Build migrator failed"
        exit 1
    }
}

foreach ($ProjectToBuild in $ServiceProjectsToBuild)
{
  BuildServiceProject($ProjectToBuild)
}

# BuildMigrator

