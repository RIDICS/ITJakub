Dependencies
------------

All projects except one in this solution uses NuGet packages for getting dependencies.
ITJakub.Web.Hub ASP.NET project using Bower for getting JavaScript dependencies and NuGet for binary dependencies.


Installing new dependencies
---------------------------
1) Add dependency to bower.json file
2) Include file in Visual Studio project
	a) Click on "Show All Files" in Solution Explorer
	b) Right click on new files and click "Include In Project"


Restoring
---------

NuGet packages are restored during build or in NuGet manager.

Bower packages are restered automatically when Visual Studio opening project/solution.
Another option is run command "bower install" in command line.

Third option is add NuGet package MSBuild.Bower and Bower packages then will be restored before each build.


Requirements
------------

Bower requires installed Git.
