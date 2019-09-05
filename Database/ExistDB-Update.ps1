# PowerShell script for upadting Elasticsearch indices

param (
    [string]$url = "xmldb:exist://localhost:8080/exist/xmlrpc",
    [string]$path = "ExistDB",
    [switch]$recreateMode = $false,
    [string]$collectionName = "jacob",
    [string]$username = "admin",
    [string]$password = "admin",
    [string]$installationPath = $null
)

$DefaultExistDbLocations = @(
    "C:\Tools\eXist-db",
    "D:\Tools\eXist-db",
    "D:\eXist-db",
    "C:\eXist-db"
)

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

#$ErrorActionPreference = "Stop"

function checkExitCode {
    param (
        [int] $code
    )

    if ($code -ne 0) {
        Write-Error "Failed with error code ${code}"
        exit 1
    }
}

# find eXist-db installation path
$existHome = $installationPath

if ($existHome -eq "")
{
    foreach ($location in $DefaultExistDbLocations)
    {
        $existHome = $location
        if (Test-Path $location)
        {
            break
        }
    }
}


if ($recreateMode) {
    Write-Host "Recreating eXist-db for collection ${collectionName} on ${url}"
}
else {
    Write-Host "Updating eXist-db for collection ${collectionName} on ${url}"
}

$scriptPath = Join-Path $CurrentPath $path

Write-Host "--------------------"

if ($recreateMode) {
    # delete old data
    java "-Dexist.home=${existHome}" -jar "${existHome}\start.jar" client "-ouri=${url}" -u $username -P $password -c "/db/apps" -R $collectionName
    checkExitCode $LASTEXITCODE
    Write-Host "--------------------"
}

# upload new data
java "-Dexist.home=${existHome}" -jar "${existHome}\start.jar" client "-ouri=${url}" -u $username -P $password -d -m "/db/apps/${collectionName}" -p "${scriptPath}"
checkExitCode $LASTEXITCODE
Write-Host "--------------------"

if ($recreateMode) {
    # delete old configuration data
    java "-Dexist.home=${existHome}" -jar "${existHome}\start.jar" client "-ouri=${url}" -u $username -P $password -c "/db/system/config/db/apps" -R $collectionName
    checkExitCode $LASTEXITCODE
    Write-Host "--------------------"
}

# upload configuration data
java "-Dexist.home=${existHome}" -jar "${existHome}\start.jar" client "-ouri=${url}" -u $username -P $password -d -m "/db/system/config/db/apps/${collectionName}" -p "${scriptPath}\config"
checkExitCode $LASTEXITCODE
Write-Host "--------------------"

# upload files for jacob-develop collection
if ($collectionName -eq "jacob-develop") {
    $scriptPathForDevelopment = "${scriptPath}ForDevelopment"
    java "-Dexist.home=${existHome}" -jar "${existHome}\start.jar" client "-ouri=${url}" -u $username -P $password -d -m "/db/apps/${collectionName}" -p $scriptPathForDevelopment
    checkExitCode $LASTEXITCODE
    Write-Host "--------------------"
}


Write-Host "--------------------"
Write-Host "Collection ${collectionName} on ${url} updated or recreated"

Write-Host
Write-Host
