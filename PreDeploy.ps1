#!/usr/bin/env pwsh

param (
	[bool]$recreateDatabases = $true
)

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host


$DatabaseFolderPath = Join-Path $CurrentPath "Database"
Set-Location $DatabaseFolderPath

$ElasticSearchScript = "Elasticsearch-Update.ps1"
$ElasticScripthPath = Join-Path $DatabaseFolderPath $ElasticSearchScript
Write-Host "Running script  ${ElasticSearchScript}"

if($recreateDatabases)
{
    & $ElasticScripthPath -recreateMode
}
else {
	& $ElasticScripthPath
}

$ExistDbScript = ""
if($recreateDatabases)
{
    $ExistDbScript = "ExistDB-Recreate.cmd"
}
else {
    $ExistDbScript = "ExistDB-Update.cmd"
}

$ExistDbScriptPath = Join-Path $DatabaseFolderPath $ExistDbScript
Write-Host "Running script  ${ExistDbScript}"
& $ExistDbScriptPath

Set-Location $CurrentPath