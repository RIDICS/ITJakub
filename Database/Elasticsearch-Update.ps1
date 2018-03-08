# PowerShell script for upadting Elasticsearch indices

param (
    [string]$url = "http://localhost:9200",
	[string]$path = "Elasticsearch",
	[switch]$recreateMode = $false,
	[string]$indexSuffix = ""
)

$ErrorActionPreference = "Stop"

Write-Host "Starting script for updating Elasticsearch indices"

Write-Host "Getting files from folder $($path)"

$files = Get-ChildItem $path

foreach ($file in $files)
{
	Write-Host $file;
}
Write-Host "Total count: $($files.Count)"
Write-Host "-----"

foreach ($file in $files)
{
	$indexName = "$($file.BaseName)$($indexSuffix)";
	$requestUrl = "$($url)/$($indexName)";
	
	if ($recreateMode)
	{
		Write-Host "Invoking method: DELETE $($requestUrl)";
		
		try {
			Invoke-RestMethod -Method Delete -Uri $requestUrl;
		} catch {
			Write-Warning "Error: can't delete index with name: $($indexName)";
		}
	}
	
	Write-Host "Invoking method: PUT $($requestUrl)";
	
	Invoke-RestMethod -Method Put -Uri $requestUrl -InFile $file.FullName;
}

Write-Host "-----Update finished-----"
