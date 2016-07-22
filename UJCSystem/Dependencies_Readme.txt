Dependencies
------------

All projects except one in this solution uses NuGet packages for getting dependencies.
ITJakub.Web.Hub ASP.NET project using Bower for getting JavaScript dependencies and NuGet for binary dependencies.

NuGet packages are restored during build or in NuGet manager.

Bower packages are restered automatically when Visual Studio opening project/solution.
Another option is run command "bower install" in command line.

Bower requires installed Git.
