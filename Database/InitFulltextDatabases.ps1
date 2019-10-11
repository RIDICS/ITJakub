#!/usr/bin/env pwsh

param (
    [string]$elasticSearchInstallationPath = $null,
    [switch]$recreateDatabases = $false,
    [string]$elasticSearchUrl = "localhost:9200",
    [string]$existDbUrl = "localhost:8080/exist"
)

$DefaultElasticLocations = @(
    "C:\Tools\Elastic\Elasticsearch",
    "D:\Tools\Elastic\Elasticsearch",
    "C:\Program Files\Elastic\Elasticsearch"
)

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host

# Verify Java is installed

try {
    $JavaVersion = (Get-Command java | Select-Object -ExpandProperty Version).toString()    
    Write-Host "Java is installed. Vesrion: ${JavaVersion}"
}
catch {
    Write-Error "Java is not installed"
}

# Verify Elasticsearch is installed with required plugin

if ($elasticSearchInstallationPath -eq "")
{
    foreach ($location in $DefaultElasticLocations)
    {
        $elasticSearchInstallationPath = $location
        if (Test-Path $location)
        {
            break
        }
    }
}

if(Test-Path $elasticSearchInstallationPath)
{
    Set-Location $elasticSearchInstallationPath
    try {
        $Command = Get-Command .\bin\elasticsearch-plugin
        if ($Command -eq $null)
        {
            Throw
        }
    }
    catch {
        Write-Error "ElasticSearch is not installed in ""${elasticSearchInstallationPath}""."
		Set-Location $CurrentPath
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

# Verify Elasticsearch and eXist-DB are installed

$elasticSearchUrl = AddScheme $elasticSearchUrl
$ExistDbTempUrl = AddScheme $existDbUrl

Write-Host ""
Write-Host "Testing connections"
TestConnection "ElasticSearch" -url $elasticSearchUrl
TestConnection "eXist-db" -url  $ExistDbTempUrl

# Create or recreate Elasticsearch indices

Write-Host ""
if ($recreateDatabases)
{
    Write-Host "Recreate fulltext databases (indices and xqueries)"
}
else
{
    Write-Host "Create fulltext databases (indices and xqueries)"
}

$ElasticSearchScript = "Elasticsearch-Update.ps1"
$ElasticScripthPath = Join-Path $CurrentPath $ElasticSearchScript
Write-Host "Running script  ${ElasticSearchScript}"

if($recreateDatabases)
{
    & $ElasticScripthPath -url $elasticSearchUrl -recreateMode
}
else {
    & $ElasticScripthPath -url $elasticSearchUrl
}

# Create or create eXist-DB xqueries

$ExistDbScript = "ExistDB-Update.ps1"
$ExistDbScriptPath = Join-Path $CurrentPath $ExistDbScript
Write-Host "Running script  ${ExistDbScript}"
$ExistScheme = "xmldb:exist" 
$ExistDbTempUrl = AddScheme $existDbUrl -scheme $ExistScheme
$ExistDbTempUrl = $ExistDbTempUrl + "/xmlrpc"
if ($recreateDatabases)
{
    & $ExistDbScriptPath -url $ExistDbTempUrl -recreateMode
}
else
{
    & $ExistDbScriptPath -url $ExistDbTempUrl
}

# Reset current location

Set-Location $CurrentPath