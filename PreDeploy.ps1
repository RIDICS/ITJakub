#!/usr/bin/env pwsh

param (
    [bool]$recreateDatabases = $true,
    [string]$elasticSearchUrl = "localhost:9200",
    [string]$existDbUrl = "localhost:8080/exist"
)

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host

try {
    $javaVersion = (Get-Command java | Select-Object -ExpandProperty Version).toString()    
    Write-Host "Java is installed. Vesrion: ${javaVersion}"
}
catch {
    Write-Error "Java is not installed"
}

function TestConnection {
    Param (
        [String]$serviceName,
        [String]$url)

    $request = [Net.HttpWebRequest]::Create($url)  

    try 
    {     
        $response = [Net.HttpWebResponse]$request.GetResponse() 
        Write-Host "Database ${serviceName} is running"
        $request.Abort() 
    }     
    Catch  
    { 
        Write-Error "Database ${serviceName} on url ${url} does not run" 
        exit 1
    }
}

$httpScheme = "http"

function AddScheme {
    param (
        [string] $url,
        [string] $scheme = $httpScheme       
    )

    $seperator = "://"

    if(-Not($url.Contains($seperator)))
    {
        return $scheme + $seperator + $url 
    }

    if($url.Contains($scheme))
    {
       return $url
    }

    $url = $url.Remove(0, $url.IndexOf($seperator))
    return $scheme + $url 
}

$elasticSearchUrl = AddScheme $elasticSearchUrl
$existDbTempUrl = AddScheme $existDbUrl

Write-Host "Testing connections"
TestConnection "ElasticSearch" -url $elasticSearchUrl
TestConnection "eXist-db" -url  $existDbTempUrl


$DatabaseFolderPath = Join-Path $CurrentPath "Database"
Set-Location $DatabaseFolderPath

$ElasticSearchScript = "Elasticsearch-Update.ps1"
$ElasticScripthPath = Join-Path $DatabaseFolderPath $ElasticSearchScript
Write-Host "Running script  ${ElasticSearchScript}"

if($recreateDatabases)
{
    & $ElasticScripthPath -url $elasticSearchUrl -recreateMode
}
else {
	& $ElasticScripthPath -url $elasticSearchUrl
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
$existScheme = "xmldb:exist" 
$existDbTempUrl = AddScheme $existDbUrl -scheme $existScheme
$existDbTempUrl = $existDbTempUrl + "/xmlrpc"
& $ExistDbScriptPath $existDbTempUrl

Set-Location $CurrentPath