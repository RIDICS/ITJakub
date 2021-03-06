#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath
$WorkingDirectory = Join-Path $CurrentPath -ChildPath "UJCSystem" | Join-Path -ChildPath "ITJakub.Web.Hub" | Join-Path -ChildPath "wwwroot"
$cssPath = Join-Path $WorkingDirectory -ChildPath "css"

Write-Output "Using theme working directory: $WorkingDirectory"

function setConfig {
  param (
    $SelectedTheme
  )

  Set-Variable -Name SelectedTheme -Value $SelectedTheme -Scope Global
}

function setDefaultConfig {
  Set-Variable -Name SelectedTheme -Value "ResearchPortal" -Scope Global
}

function getThemeFile {
  param (
    $SelectedTheme
  )

  return Join-Path $cssPath "ITJakub.Colors.$SelectedTheme.less"
}
function getImagesFile {
  param (
    $SelectedTheme
  )

  return Join-Path $cssPath "ITJakub.Images.$SelectedTheme.less"
}

function copyLessFile {
  param (
    [string]$OutputFileName,
    [string]$SourceFilePath
  )
  
  $OutputFilePath = Join-Path $cssPath $OutputFileName
  Write-Output "Generating: ${OutputFilePath}"
  
  # Set-Content sometimes fails with “Stream was not readable”, so it is replaced by [System.IO.File]
  # Set-Content -Path $OutputFilePath -Value "//-----Generated content-----`r`n" -Force -ErrorAction Stop
  [System.IO.File]::WriteAllText($OutputFilePath, "//-----Generated content-----`r`n")
  
  Add-Content -Path $OutputFilePath -Value (Get-Content -Path $SourceFilePath) -Force -ErrorAction Stop
}

if ($Args.Count -gt 0) {
  setConfig $Args.get(0)
}
else {
  setDefaultConfig
}

$themeFile = getThemeFile $SelectedTheme
if (! (Test-Path -Path $ThemeFile)) {
  Write-Warning "$SelectedTheme theme not found, using default configuration"
  setDefaultConfig

  $themeFile = getThemeFile $SelectedTheme
}

$imagesFile = getImagesFile $SelectedTheme

Write-Output $themeFile
Write-Output $imagesFile

copyLessFile -OutputFileName "ITJakub.Colors-selected.less" -SourceFilePath $themeFile
copyLessFile -OutputFileName "ITJakub.Images-selected.less" -SourceFilePath $imagesFile