#!/usr/bin/env pwsh

param (
    [string]$elasticSearchInstallationPath = "C:Program Files\Elastic\Elasticsearch",
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
    $JavaVersion = (Get-Command java | Select-Object -ExpandProperty Version).toString()    
    Write-Host "Java is installed. Vesrion: ${JavaVersion}"
}
catch {
    Write-Error "Java is not installed"
}

if(Test-Path $elasticSearchInstallationPath)
{
    Set-Location $elasticSearchInstallationPath
    try {
        $Command = Get-Command .\bin\elasticsearch-plugin
    }
    catch {
        Write-Error "ElasticSearch is not installed in ""${elasticSearchInstallationPath}""."
        exit 1
    }

    $elasticSearchPlugins = & .\bin\elasticsearch-plugin list    
    if($elasticSearchPlugins.Contains("experimental-highlighter"))
    {
        "experimental-highlighter-elasticsearch-plugin is installed"
    }
    else {
        Write-Host "Installing experimental-highlighter-elasticsearch-plugin"
        & ./bin/elasticsearch-plugin install org.wikimedia.search.highlighter:experimental-highlighter-elasticsearch-plugin:5.5.2.2
    }
    Set-Location $CurrentPath
}
else {
    Write-Error "ElasticSearch installation folder ""${elasticSearchInstallationPath}"" is not valid."
    exit 1
}

    

function TestConnection {
    Param (
        [String]$serviceName,
        [String]$url)

    $Request = [Net.HttpWebRequest]::Create($url)  

    try 
    {     
        $Response = [Net.HttpWebResponse]$Request.GetResponse() 
        Write-Host "Database ${serviceName} is running"
        $Request.Abort() 
    }     
    Catch  
    { 
        Write-Error "Database ${serviceName} on url ${url} does not run" 
        exit 1
    }
}

$HttpScheme = "http"

function AddScheme {
    param (
        [string] $url,
        [string] $scheme = $HttpScheme       
    )

    $Seperator = "://"

    if(-Not($url.Contains($Seperator)))
    {
        return $scheme + $Seperator + $url 
    }

    if($url.Contains($scheme))
    {
       return $url
    }

    $url = $url.Remove(0, $url.IndexOf($Seperator))
    return $scheme + $url 
}

$elasticSearchUrl = AddScheme $elasticSearchUrl
$ExistDbTempUrl = AddScheme $existDbUrl

Write-Host "Testing connections"
TestConnection "ElasticSearch" -url $elasticSearchUrl
TestConnection "eXist-db" -url  $ExistDbTempUrl


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
$ExistScheme = "xmldb:exist" 
$ExistDbTempUrl = AddScheme $existDbUrl -scheme $ExistScheme
$ExistDbTempUrl = $ExistDbTempUrl + "/xmlrpc"
& $ExistDbScriptPath $ExistDbTempUrl

Set-Location $CurrentPath