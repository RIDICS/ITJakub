# Vokabulář webový

## Setup developer computer

Required software:
* Microsoft Windows
* Microsoft Visual Studio 2017 / 2019
  * ASP.NET and web development
  * .NET desktop development (for BatchImport client app)
  * .NET Core cross-platform development
* Microsoft SQL Server
* Java runtime
* eXist-db 2.1
* .NET Core 2.2 SDK
* Altova XML 2013 Community Edition (installer is in repository - Resources folder)
* Enable following Windows features (from Turn Windows features on or off dialog):
  * Internet Information Services
  * Internet Information Services Hostable Web Core
  * .NET Framework 4.x Advanced Services > WCF Services > HTTP activation
* Yarn package manager
* Git - for restoring NPM/Yarn dependencies
* (Node.js - used by Authentication service for restoring NPM/Yarn)
* Elasticsearch 5.5.2

Recommended software:
* JetBrains ReSharper
* SQL Management Studio
* Yarn Installer Visual Studio Extension

Environment configuration
* Checkout repository to C:\Pool\itjakub\
* Checkout itjakub-secrets repository to C:\Pool\itjakub-secrets\
* Checkout Authentication repository (from organization RIDICS) to the disk and setup it according to it's Readme file (ITJakub.Web.Hub won't start without running Authentication service)
* Checkout YAFNET repository (from organization RIDICS) to the disk and setup it according to it's Readme file (Vokabular defaultly creates a new forum for each imported book)
* Configure connection strings and passwords in itjakub-secrets folder
* Create database schema using Vokabular.Database.Migrator app - the only step is to run this application which automatically creates following databases:
  * VokabularWebDB - database for ITJakub.Web.Hub for storing texts
  * VokabularDB - database for Vokabular.MainService
* Other databases which should be created automatically from Forum and Authentication setup are following:
  * VokabularForumDB - database for Forum
  * VokabularAuthDB - database for Authentication service
* Configure JAVA_HOME environment variable to Java folder
* Run script `Database\InitFulltextDatabases.ps1` to check if fulltext databases are correctly installed and initialize them. The script will do following steps:
  * Check if eXist-db is correctly installed and running
  * Check if Elasticsearch is correctly installed and running
  * Check if Elasticsearch has installed required plugin or install it (Experimental highlighter)
  * Upload required resources to eXist-db (xqueries)
  * Create indices in Elasticsearch
* Restore Yarn dependencies (for development) using `YarnInstall.ps1` script.
* Run `SelectPortalTheme.{SELECTED_PORTAL}.ps1` to choose portal style otherwise build fails because of missing styles. Run this script any time you want to change a theme.

Fulltext databases manual initialization (instead of running `InitFulltextDatabases.ps1` script)
* Prepare eXist-db collection (it's possible either to use script `ExistDB-Update.ps1` or copy ExistDB folder content manually).
  * Automatic script use predefined default values or values specified as parameters:
    1. -url eXist URL (default is xmldb:exist://localhost:8080/exist/xmlrpc)
    2. -path Scripts path on disk (default is C:\Pool\itjakub\Database\ExistDB)
    3. -collectionName Collection name (default is jacob)
    4. -username Username
    5. -password Password
	6. -recreateMode Determine if DB should be deleted and then created new (default is $false)
	7. -installationPath Path where eXist-db is installed (default will try some predefined paths)
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

Optional environment configuration
* Install certificates (required only if testing deployment to IIS)
  1. Open Manager for computers certificates - certlm.msc
  2. Install ITJakubCA to Trusted Root Certification Authrorities for Local computer
  3. Install certificate ITJakubService.pfx to Personal store in Local computer
  4. ~~Click on each certificate in personal store and select "Manage private keys" and add "Everyone" for full control to all certificates~~
* Allow SSL in IIS (required only if testing deployment to IIS)
  1. In IIS manager select website and add HTTPS binding with "localhost" certificate issued by ITJakubCA
  2. Ensure that selected website ignores client certificates. This setting can be found in SSL Settings. If client certificate is enabled, the large file upload probably won't work.

Notes
> All NuGet and Yarn (only runtime) dependencies are automatically restored by Visual Studio.

> *It is highly recommended to disable automatic NPM and Bower restore in Visual Studio and use Yarn instead of that.* The whole project is configured to Yarn.

Services to deploy:
* ITJakub.Web.Hub - web portal (ASP.NET Core) with two modes available (Research and Community)
* Vokabular.MainService - main service for direct client communication (ASP.NET Core)
* Vokabular.FulltextService - service for searching in fulltext database of Community portal (in Elasticsearch) (ASP.NET Core)
* ITJakub.FileProcessing.Service - service for importing books from DOCX format (WCF service)
* ITJakub.SearchService - service for searching in fulltext database of Research portal (in eXist-db) (WCF service)
* ITJakub.Lemmatization.Service - service for lemmatization (WCF service)

Desktop apps:
* ITJakub.BatchImport.Client - application for importing multiple books in DOCX format to database

WCF services are deployed to IIS, so Visual Studio requires Administrator permission.
ASP.NET Core services are deployed to IIS Express for development purposes.

## Setup for server

Required software:
* Microsoft Windows Server
* Microsoft SQL Server
  * SQL Management Studio is also recommended
* Java runtime
* eXist-db 2.1
* .NET Core 2.2 with Windows Hosting Bundle
* Altova XML 2013 Community Edition (installer is in this repository - Resources folder)
* Add following roles or features (using Add Role an Features Wizard):
  * Role: Web Server (IIS)
  * Feature: IIS Hostable Web Core
  * Feature: .NET Framework 4.x Features > WCF Services > HTTP activation
* Elasticsearch 5.5.2
* Web Platform Installer (WebPI)
* Web Deploy (MsDeploy) from WebPI

Environment configuration on build computer
* Checkout repositories as described in *Setup developer computer*
* Create configuration files in itjakub-secrets folder (it is desribed in following chapter: *Configuration for different environments*)

Server environment configuration
* Setup Authentication service (from organization RIDICS) according to it's readme
* Setup Forum YAFNET (from organization RIDICS) according to it's readme
* Configure Application Pools in IIS
  1. Create new Application Pool (e.g. .NET Core) with ".NET CLR version" set to "No Managed Code"
  2. Configure ASP.NET Core services to use .NET Core Application Pool (every ASP.NET Core service run as separate process with Kestrel server)
  3. Optionally configure this Application Pool to disable supending (app will be always running)
* Add new Website in IIS (used for internal services)
  * Name: LocalhostServices
  * Application Pool: (created in previous step)
  * Type: HTTP, port: 85, host name: localhost (restrict access only from localhost)
* Configure JAVA_HOME environment variable to Java folder
* Copy Database folder from itjakub folder to the server and run script `InitFulltextDatabases.ps1` to check if fulltext databases are correctly installed and initialize them.
* Build and deploy the app as described in *The project build and deployment* section

## Configuration for different environments

Solution is configured to load sensitive information from external location which is C:\Pool\itjakub-secrets
The reason of separating this info from the remaining code is avoiding accidental commit of private/secret information to public git.
The template of this configuration can be checked out from https://github.com/RIDICS/itjakub-secrets

**ASP.NET Core projects** are configured to use specific configuration files for different environments.
Selected environment is configured in globalsettings.json file. Default configuration is stored in appsettings.json file.
Environment specific configuration is stored in appsettings.{ENVIRONMENT_NAME}.json file.
Currently there exists three preconfigured environments:
* default (without name) - shared values for all configurations and default endpoint URLs
* LocalDebug - endpoint URLs configured to use services deployed to IIS Express
* Development - configuration for deploying testing instance to server (on URL http://{server}/Development)

Summary - the complete configuration is consist of all following settings files (new file overwrites only values which are specified in this new file):
* globalsettings.json selects ENVIRONMENT_NAME
* appsettings.json is always loaded
* configuration is changed to values specified in appsettings.{ENVIRONMENT_NAME}.json if the file exists
* configuration is changed to values specified in C:\Pool\itjakub-secrets\ITJakub.Secrets.json
* configuration is changed to values specified in C:\Pool\itjakub-secrets\ITJakub.Secrets.{ENVIRONMENT_NAME}.json if the file exists

**WCF Service projects** use specific Web.config files transformed by Web.{ENVIRONMENT_NAME}.config during the build process according to selected build configuration.
* Web.{ENVIRONMENT_NAME}.config is loaded automatically either from the project folder (e.g. C:\Pool\itjakub\UJCSystem\ITJakub.Lemmatization.Service) or from C:\Pool\itjakub-secrets\WcfServices
* Web.{ENVIRONMENT_NAME}.config usually assumes loading secrets from C:\Pool\itjakub-secrets\WcfServices (ITJakub.Secrets.config or ITJakub.Secrets.{ENVIRONMENT_NAME}.config)

The setting from itjakub-secrets are included in deployment package, so the server doesn't need any more steps for setup the configuration.

**Authentication Service project**
* Authentication Service includes configuration from C:\Pool\itjakub-secrets\Auth (it requires appsettings.{ENVIRONMENT_NAME}.json5 and modules.{ENVIRONMENT_NAME}.json5)
* Database Migrator includes configuration from C:\Pool\itjakub-secrets\DatabaseMigratorAuth (it requires appsettings.{ENVIRONMENT_NAME}.json)

**Forum (YAFNET) project**
The configuration must be created directly in the project folder (according to Readme file) and MUST NOT be commited to Git.

## The project build and deployment

* Ensure that the latest version of Authentication service and Forum is deployed
* Choose environment which you want to build (which settings will be used)
* Run script `BuildSolution.ps1 {ENVIRONMENT_NAME}`
* All build artifacts are created in `build\Publish-{ENVIRONMENT_NAME}` folder
* Copy this folder to target server
* Optionally update `{SERVICE_NAME}.SetParameters.xml` files with desired target path (site) in IIS
* Run script `DeploySolution.ps1`
  * This script run DB Migrator to update database schema, deploy the services and configure permissions to logs folders (allow logging).
  * -disableInteractive this parameter disable waiting for user input after each service deployment
  * -test run deployment simulation, it doesn't deploy anything but it creates a report

### Intended deploy model

Inteded deployment model is following:
* ITJakub.Web.Hub (research mode) deployed as example.com
* ITJakub.Web.Hub (community mode) deployed as example.com/Community or community.example.com
* Vokabular.MainService deployed as example.com/MainService
* Ridics.Authentication.Service deployed as example.com/Auth
* Ridics YAFNET Forum deployed as example.com/Forum
* ITJakub.FileProcessing.Service deployed as localhost:85/ITJakub.FileProcessing.Service
* ITJakub.Lemmatization.Service deployed as localhost:85/ITJakub.Lemmatization.Service
* ITJakub.SearchService deployed as localhost:85/ITJakub.SearchService
* Vokabular.FulltextService deployed as localhost:85/Vokabular.FulltextService

**example.com is "Default Web Site" in IIS**
* Publicly available over some host name e.g. example.com
* HTTPS secured by valid SSL certificate

**localhost:85 is "LocalhostServices" site in IIS**
* HTTP running on port 85
* Available only for localhost by specified host name "localhost"

## Default credentials

**Solution is prepared to use default credentials. It is highly recommended to change these values for deployed application.**

Sensitive credentials for accessing to databases or third party services are stored in separated folder (itjakub-secrets).
These values should be replaced with secret ones and the folder should not be accessible to users.

**Default login to portal should be also changed** because has complete (administrator) permissions.

Login to portal: Admin
Default password: administrator
Description: This user has Admin role which is a special role used for Authentication service configuration. This user and this role are hidden in the Web.Hub and doesn't have full functionality available.

Login to portal: PortalAdmin
Default password: administrator
Description: This user has PortalAdmin role which is used for managing portal Web.Hub and for managing created roles/groups and theirs permissions.

## Securing services

Currently secured services are:
* ITJakub.Web.Hub - web portal using Authentication service
* Vokabular.MainService - main service, secured by access token from Authentication service

**Other services are not intended for direct client communication, so they should be accessible only for MainService or Web.Hub.**
The default configuration assumes deployment of these services to IIS Site which is configured only for access from http://localhost:85 address.


## Common errors

**Install Elasticsearch plugin failed with error: The syntax of the command is incorrect.**  
Java is not installed or Java folder is missing in PATH variable.

**IIS error 500.19 - The requested page cannot be accessed because the related configuration data for the page is invalid.**  
Check if ".NET Core Windows Server Hosting" is installed.

**An error occurred while starting the application.**  
* Check relational database access configuration (username, password, database exists, ...)
* If you change stdoutLogEnabled in web.config to true, specific error can be displayed in text file inside "logs" folder.

**Logging doesn't work on IIS.**  
System user IIS_IUSRS must has permission to write to "logs" folder.

**The large file upload fails with 413.0 - Request Entity Too Large on IIS server.**
The problem may be caused by enabled client certificate. Try to set "Ignore client certificate" in SSL Settings in IIS Manager.

**Random errors when running `InitFulltextDatabases.ps1`.**
Ensure that JAVA_HOME environment variable is set to correct Java installation folder.

**eXist-db installed as service won't start with error "The eXist-db Native XML Database service terminated unexpectedly".**
The problem may be caused by wrong path to Java installation folder. Ensure that JAVA_HOME environment variable is set and also ensure that correct path is specified in C:\eXist-db\tools\wrapper\conf\wrapper.conf configuration file.

**`gulp default` fails when building ITJakub.Web.Hub.**
Ensure that correct node modules has been installed (run `yarn install`). Then ensure that gulpfile.js was loaded successfully (output in Task Runner Explored window in Visual Studio).

> You can also check Troubleshooting section in Readme file of Authentication Service
