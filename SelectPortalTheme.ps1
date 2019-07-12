#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath
$WorkingDirectory = Join-Path $CurrentPath -ChildPath "UJCSystem" | Join-Path -ChildPath "ITJakub.Web.Hub" | Join-Path -ChildPath "wwwroot"
$cssPath = Join-Path $WorkingDirectory -ChildPath "css"
$imagesPath = Join-Path $WorkingDirectory -ChildPath "images"
$logoPath = Join-Path $WorkingDirectory -ChildPath "images" | Join-Path -ChildPath "logo"


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

  return Join-Path $cssPath "ITJakub.$SelectedTheme.Colors.less"
}
function getImagesFile {
  param (
    $SelectedTheme
  )

  return Join-Path $cssPath "ITJakub.$SelectedTheme.Images.less"
}
function getLogoFile {
  param (
    $SelectedTheme
  )

  return Join-Path $logoPath "logo.$SelectedTheme.png"
}
function getFaviconFile {
  param (
    $SelectedTheme
  )

  return Join-Path $imagesPath "favicon.$SelectedTheme.png"
}
function copyLessFile {
  param (
    [string]$OutputFileName,
    [string]$SourceFilePath
  )
  
  Set-Content -Path (Join-Path $cssPath $OutputFileName) -Value "//-----Generated content-----`r`n"
  Add-Content -Path (Join-Path $cssPath $OutputFileName) -Value (Get-Content -Path $SourceFilePath)
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
$logoFile = getLogoFile $SelectedTheme
$faviconFile = getFaviconFile $SelectedTheme

Write-Output $themeFile
Write-Output $imagesFile
Write-Output $logoFile
Write-Output $faviconFile

copyLessFile -OutputFileName "ITJakub.PortalSpecific.Colors.less" -SourceFilePath $themeFile
copyLessFile -OutputFileName "ITJakub.PortalSpecific.Images.less" -SourceFilePath $imagesFile
Copy-Item -Force $logoFile (Join-Path $logoPath "logo.png")
Copy-Item -Force $faviconFile(Join-Path $imagesPath "favicon.png")
