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

Write-Host
Write-Host
Write-Host "DEPLOYMENT FINISHED"
Write-Host
