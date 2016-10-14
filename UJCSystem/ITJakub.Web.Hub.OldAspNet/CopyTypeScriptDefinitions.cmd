@echo off

echo Copying definitely typed files to project folder

xcopy /e /v /y %USERPROFILE%\.nuget\packages\bootstrap.TypeScript.DefinitelyTyped\0.9.2\Content\Scripts\typings wwwroot\js\typings\
xcopy /e /v /y %USERPROFILE%\.nuget\packages\dropzone.TypeScript.DefinitelyTyped\1.3.4\Content\Scripts\typings wwwroot\js\typings\
xcopy /e /v /y %USERPROFILE%\.nuget\packages\jquery.TypeScript.DefinitelyTyped\3.1.0\Content\Scripts\typings wwwroot\js\typings\
xcopy /e /v /y %USERPROFILE%\.nuget\packages\jqueryui.TypeScript.DefinitelyTyped\1.4.8\Content\Scripts\typings wwwroot\js\typings\
xcopy /e /v /y %USERPROFILE%\.nuget\packages\simplemde.TypeScript.DefinitelyTyped\0.0.1\Content\Scripts\typings wwwroot\js\typings\
xcopy /e /v /y %USERPROFILE%\.nuget\packages\typeahead.TypeScript.DefinitelyTyped\0.3.5\Content\Scripts\typings wwwroot\js\typings\

echo Finished
