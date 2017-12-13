# Vokabulář webový

## Setup developer computer

Required software:
* Microsoft Windows
* Microsoft Visual Studio 2017
* Microsoft SQL Server
* eXist-db 2.1
* .NET Core 2.0 SDK
* TypeScript 2.4 SDK
* Altova XML 2013 Community Edition (installer is in repository)
* Internet Information Services (installed from Windows features dialog)

Recommended software:
* JetBrains ReSharper
* SQL Management Studio

Environment configuration
* Checkout repository to C:\Pool\itjakub\
* Checkout itjakub-secrets repository to C:\Pool\itjakub-secrets\
* Configure connection strings and passwords in itjakub-secrets folder
* Create database schema (using create scripts in Database folder):
  * ITJakubDB - old database for ITJakub.ITJakubService (will be removed)
  * ITJakubWebDB - database for ITJakub.Web.Hub for storing texts
  * VokabularDB - new database for Vokabular.MainService
* Prepare eXist-db collection (it's possible use script ExistDB-Recreate.cmd or copy ExistDB folder content manually - instructions are in Deploy_readme.txt)
* Install certificates and allow SSL in IIS (instructions are in Deploy_readme.txt)

All dependencies are automatically restored by Visual Studio:
* NuGet
* NPM
* Bower

Services to deploy:
* ITJakub.Web.Hub - web portal (ASP.NET Core)
* Vokabular.MainService - new main service for direct client communication (ASP.NET Core)
* Vokabular.FulltextService - service for searching in fulltext database (in ElasticSearch) (ASP.NET Core)
* ITJakub.ITJakubService - original main service (will be completetly replaced by Vokabular.MainService) (WCF service)
* ITJakub.FileProcessing.Service - service for importing books from DOCX format (WCF service)
* ITJakub.SearchService - service for searching in fulltext database using old format (in eXist-db) (WCF service)
* ITJakub.Lemmatization.Service - service for lemmatization (WCF service)

WCF services are deployed to IIS, so Visual Studio requires Administrator permission.
ASP.NET Core services are deployed to IIS Express for development purposes.
