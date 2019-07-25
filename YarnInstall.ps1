#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

$item = "UJCSystem\ITJakub.Web.Hub"

$FailedCount = 0

Write-Host
Write-Host $item
Set-Location $item

yarn install

if ($LASTEXITCODE -ne 0)
{
    $FailedCount++
}

yarn audit

Write-Host

$MessageColor = "green"
if ($FailedCount -gt 0)
{
    $MessageColor = "red"
}
Write-Host "1 Yarn project attempted to install," $FailedCount "failed" -foregroundcolor $MessageColor

cd $CurrentPath

Write-Host

if ($FailedCount -gt 0)
{
    exit 1
}
