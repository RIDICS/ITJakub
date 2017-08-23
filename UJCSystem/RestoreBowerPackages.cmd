@echo off

set CI=true
set SOLUTION_PATH=%~dp0
pushd %SOLUTION_PATH%

echo Restoring Bower and NPM packages

rem specify "cd" and "bower" command for all projects with Bower dependecies
cd ITJakub.Web.Hub
call npm install
call bower install --config.interactive=false

rem ----------

echo Restored

popd
