@echo off

set CI=true
pushd %CD%

echo Restoring Bower packages

rem specify "cd" and "bower" command for all projects with Bower dependecies
cd ITJakub.Web.Hub
call bower install --config.interactive=false

rem ----------

echo Restored

popd
