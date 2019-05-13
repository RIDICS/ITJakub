# Vokabulář webový

## Setup developer computer

Required software:
* Microsoft Windows
* Microsoft Visual Studio 2017
  * ASP.NET and web development
  * .NET desktop development (for BatchImport client app)
  * .NET Core cross-platform development
  * (Git - for restoring NPM and Bower dependencies)
  * (Node.js - for restoring NPM)
* Microsoft SQL Server
* Java
* eXist-db 2.1
* .NET Core 2.0 SDK
* TypeScript 2.4 SDK
* Altova XML 2013 Community Edition (installer is in repository)
* Internet Information Services (installed from Windows features dialog)
* Yarn package manager
* Elasticsearch 5.5.2

Recommended software:
* JetBrains ReSharper
* SQL Management Studio
* Yarn Installer Visual Studio Extension

Environment configuration
* Checkout repository to C:\Pool\itjakub\
* Checkout itjakub-secrets repository to C:\Pool\itjakub-secrets\
* Configure connection strings and passwords in itjakub-secrets folder
* Create database schema (use numbered SQL create scripts in correct order, stored in Database folder):
  * ITJakubDB - old database for ITJakub.ITJakubService (will be removed)
  * ITJakubWebDB - database for ITJakub.Web.Hub for storing texts
  * VokabularDB - new database for Vokabular.MainService
* Prepare eXist-db collection (it's possible use script ExistDB-Recreate.cmd or copy ExistDB folder content manually).
  * Automatic script use predefined default values or values specified as parameters in following order:
    1. eXist URL (default is xmldb:exist://localhost:8080/exist/xmlrpc)
    2. scripts path on disk (default is C:\Pool\itjakub\Database\ExistDB)
    3. collection name (default is jacob)
    4. username
    5. password
  * Manual file copying
    1. open eXist-db Java Admin Client
	2. create collection with name "apps/jacob"
	3. copy content of "Database/ExistDB" folder except "config" folder to app collection named "jacob"
	4. copy content of "Database/ExistDB/config" folder to collection "/system/config/db/apps/jacob"
* Prepare Elasticsearch:
  * Install Experimental highlighter plugin using following command: "./bin/elasticsearch-plugin install org.wikimedia.search.highlighter:experimental-highlighter-elasticsearch-plugin:5.5.2.2" in Elasticsearch installation directory.
  * Create indices using Elasticsearch-Update.ps1 script or manually using REST calls to Elasticsearch with configuration stored in "Database/Elasticsearch" folder (every file represents configuration for one index, index name is the same as file name).
  * Elasticsearch-Update.ps1 script has following parameters:
    1. -url URL of database (default is "http://localhost:9200")
    2. -path Folder with indices configuration (default is "Elasticsearch")
    3. -recreateMode Determine if indices should be deleted and then created new (default is $false)
    4. -indexSuffix Add suffix to index name (default is empty)
* Install certificates
  1. Open Manager for computers certificates - certlm.msc
  2. Install ITJakubCA to Trusted Root Certification Authrorities for Local computer
  3. Install certificates for ITJakubClient.pfx and ITJakubService.pfx to Personal store in Local computer
  4. Click on each certificate in personal store and select "Manage private keys" and add "Everyone" for full control to all certificates
* Allow SSL in IIS
  1. In IIS manager select website and add HTTPS binding with "localhost" certificate issued by ITJakubCA
  2. For selected website open SSL Settings and select "Accept client certificate"
* Configure Application Pools in IIS
  1. ITJakub.SearchService should be deployed in different Application Pool than ITJakub.ITJakubService

All dependencies are automatically restored by Visual Studio:
* NuGet
* NPM
* Yarn

It is possible to install Yarn Installer Extension and configure it to automatically restore Yarn dependencies and disable automatic package restore for NPM.

Services to deploy:
* ITJakub.Web.Hub - web portal (ASP.NET Core)
* Vokabular.MainService - new main service for direct client communication (ASP.NET Core)
* Vokabular.FulltextService - service for searching in fulltext database (in Elasticsearch) (ASP.NET Core)
* ITJakub.ITJakubService - original main service (will be completetly replaced by Vokabular.MainService) (WCF service)
* ITJakub.FileProcessing.Service - service for importing books from DOCX format (WCF service)
* ITJakub.SearchService - service for searching in fulltext database using old format (in eXist-db) (WCF service)
* ITJakub.Lemmatization.Service - service for lemmatization (WCF service)

Desktop apps:
* ITJakub.BatchImport.Client - application for importing multiple books in DOCX format to database

WCF services are deployed to IIS, so Visual Studio requires Administrator permission.
ASP.NET Core services are deployed to IIS Express for development purposes.

## Setup for server

Required software:
* Microsoft Windows
* Microsoft SQL Server
* eXist-db 2.1
* .NET Core 2.0 Windows Server Hosting
* Altova XML 2013 Community Edition (installer is in repository)
* Internet Information Services

Environment configuration
* Almost same as Developer computer
* Configure Application Pools in IIS
  1. Create new Application Pool (e.g. .NET Core) with ".NET CLR version" set to "No Managed Code"
  2. Configure ASP.NET Core services to use .NET Core Application Pool (every ASP.NET Core service run as separate process with Kestrel server)

## Environment configuration

**ASP.NET Core projects** are configured to use specific configuration files for different environments.
Selected environment is configured in globalsettings.json file. Default configuration is stored in appsettings.json file.
Environment specific configuration is stored in appsettings.{environment_name}.json file.
Currently exists three preconfigured environments:
* default (without name) - shared values for all configurations and default endpoint URLs
* LocalDebug - endpoint URLs configured to use services deployed to IIS Express
* Development - configuration for deploying testing instance to server (on URL http://{server}/Development)

**WCF Service projects** use specific Web.config files (Web.{build_configuration}.config) according to selected build configuration during publish process.

Path to itjakub-secrets folder can be configured in globalsettings.json or Web.config file.

## Default credentials

**Solution is prepared to use default credentials. It is highly recommended to change these values for deployed application.**

Sensitive credentials for accessing to databases or third party services are stored in separated folder (itjakub-secrets).
These values should be replaced with secret ones and the folder should not be accessible to users.

**Default login to portal should be also changed** because has complete (administrator) permissions.

Login to portal: Admin
Default password: administrator

## Securing services

Currently secured services are:
* ITJakub.Web.Hub - web portal
* Vokabular.MainService - new main service, secured by communication token
* ITJakub.ITJakubService - original main service, secured by communication token

**Other services are not intended for direct client communication, so they should be accessible only for MainService or Web.Hub.**


## Common errors

**Install Elasticsearch plugin failed with error: The syntax of the command is incorrect.**  
Java is not installed or Java folder is missing in PATH variable.


**Publish failed with: Error MSB3073: The command "npm install" exited with code 9009.**  
NPM is not added to system path.

**Publish failed with: Error MSB3073: The command "gulp clean" exited with code 9009.**  
Gulp is not installed as global package.

**IIS error 500.19 - The requested page cannot be accessed because the related configuration data for the page is invalid.**  
Check if ".NET Core Windows Server Hosting" is installed.

**An error occurred while starting the application.**  
* Check relational database access configuration (username, password, database exists, ...)
* If you change stdoutLogEnabled in web.config to true, specific error can be displayed in text file inside "logs" folder.

**Logging doesn't work on IIS.**  
System user IIS_IUSRS must has permission to write to "logs" folder.