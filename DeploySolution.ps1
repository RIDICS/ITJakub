#!/usr/bin/env pwsh

param (
	[switch]$disableInteractive = $false,
	[switch]$test = $false
)

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host

$ProjectsToDeploy = @()
$ProjectsToDeploy += "ITJakub.FileProcessing.Service"
$ProjectsToDeploy += "ITJakub.Lemmatization.Service"
$ProjectsToDeploy += "ITJakub.SearchService"
$ProjectsToDeploy += "Vokabular.FulltextService"
$ProjectsToDeploy += "Vokabular.MainService"
$ProjectsToDeploy += "ITJakub.Web.Hub-CommunityPortal\ITJakub.Web.Hub"
$ProjectsToDeploy += "ITJakub.Web.Hub-ResearchPortal\ITJakub.Web.Hub"

# Find all projects for deployment

Write-Host "Projects to deploy:"
Write-Host

$NotFoundCount = 0;
$DeploymentScripts = @()

foreach ($ProjectToDeploy in $ProjectsToDeploy)
{
  $DeployScriptPath = Join-Path $CurrentPath "${ProjectToDeploy}.deploy.cmd"
  $DeploymentScripts += $DeployScriptPath

  if (Test-Path $DeployScriptPath)
  {
    Write-Host "${ProjectToDeploy} FOUND" -foregroundcolor green
  }
  else
  {
    $NotFoundCount++
    Write-Host "${ProjectToDeploy} NOT FOUND" -foregroundcolor red
  }
}

if ($NotFoundCount -gt 0)
{
  Write-Host
  Write-Host "Some projects for deployment were not found" -foregroundcolor red
  Write-Host "Deployment cancelled" -foregroundcolor red
  Write-Host
  exit 1
}

$MigrationToRun = "Vokabular.Database.Migrator"
$MigrationScriptPath = Join-Path $CurrentPath "${MigrationToRun}\Migrate.ps1"

if (Test-Path $MigrationScriptPath)
{
  Write-Host "${MigrationToRun} FOUND" -foregroundcolor green
}
else {
  Write-Host
  Write-Host "Migrator project for running migrations was not found" -foregroundcolor red
  Write-Host "Deployment cancelled" -foregroundcolor red
  Write-Host
  exit 1
}

# DB migrator

Write-Host
Write-Host

$CurrentFolderName = (Get-Item $CurrentPath).Name
$TargetEnvironment = $CurrentFolderName.Split('-')[1]

$MigratorPath = Join-Path $CurrentPath "${MigrationToRun}"

Set-Location $MigratorPath
& $MigrationScriptPath ${TargetEnvironment}

Set-Location $CurrentPath

if ($LASTEXITCODE -ne 0)
{
  Write-Error "Mirgrations ${MigrationToRun} failed"
  exit 1
}

# App and services deployment

Write-Host
Write-Host "Starting deployment"
Write-Host
Write-Host


foreach ($DeploymentScript in $DeploymentScripts)
{
  $AdditionalArguments = "/y"
  if ($test)
  {
    $AdditionalArguments = "/t"
  }
  
  & $DeploymentScript -enableRule:DoNotDeleteRule -enablerule:AppOffline $AdditionalArguments
  
  if (-Not $disableInteractive)
  {
    Write-Host
    Write-Host $DeploymentScript
    Read-Host "  completed, press enter to continue."
    Write-Host
  }
  else
  {
    Write-Host
  }
}

# Create log folders

Write-Host
Write-Host "Checking if logs folders must be created"
Write-Host

$DefaultWebSitePath = Get-WebFilePath 'IIS:\Sites\Default Web Site'
$LocalhostServicesPath = Get-WebFilePath 'IIS:\Sites\LocalhostServices'

$ServiceFolders = @(
    $DefaultWebSitePath
    Join-Path $DefaultWebSitePath "Community"
    Join-Path $DefaultWebSitePath "MainService"
    Join-Path $LocalhostServicesPath "ITJakub.FileProcessing.Service"
    Join-Path $LocalhostServicesPath "ITJakub.Lemmatization.Service"
    Join-Path $LocalhostServicesPath "ITJakub.SearchService"
    Join-Path $LocalhostServicesPath "Vokabular.FulltextService"
)
 
function SetFullAccessToFolder {
    Param ([String]$FolderPath)

    $Acl = Get-Acl $FolderPath 

    $AccessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")

    $Acl.SetAccessRule($AccessRule)
    Set-Acl $FolderPath $Acl
}

foreach ($ServiceFolder in $ServiceFolders)
{
  $ServiceLogsPath = Join-Path $ServiceFolder "logs"
  
  Write-Host $ServiceLogsPath
  
  if (-Not (Test-Path $ServiceLogsPath))
  {
    New-Item -Path $ServiceLogsPath -ItemType "directory" > $null
    SetFullAccessToFolder($ServiceLogsPath)

    Write-Host " - Folder created and IIS_IUSRS gained permissions to this folder"
  }
  else
  {
    Write-Host " - Already created"
  }
}

Write-Host
Write-Host "Logs folders checked or created"
Write-Host

Write-Host
Write-Host
Write-Host "DEPLOYMENT FINISHED"
Write-Host