@echo off

set EXIST_URL=%1
set SCRIPT_PATH=%2
set COLLECTION_NAME=%3
set USERNAME=%4
set PASSWORD=%5

if %1.==. set EXIST_URL=xmldb:exist://localhost:8080/exist/xmlrpc
if %2.==. set SCRIPT_PATH="C:\Pool\itjakub\Database\ExistDB"
if %3.==. set COLLECTION_NAME=jacob
if %4.==. set USERNAME=admin
if %5.==. set PASSWORD=admin

set EXIST_HOME="C:\eXist-db"


echo Recreating eXist-db for collection %COLLECTION_NAME% on %EXIST_URL%
echo --------------------

rem delete old data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -c /db/apps -R %COLLECTION_NAME% || goto error
echo --------------------

rem upload new data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH% || goto error
echo --------------------

rem delete old configuration data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -c /db/system/config/db/apps -R %COLLECTION_NAME% || goto error
echo --------------------

rem upload configuration data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/system/config/db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH%\config || goto error
echo --------------------

rem upload files for jacob-develop collection
if "%COLLECTION_NAME%" == "jacob-develop" (
  java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH%ForDevelopment || goto error
)

echo --------------------
echo Collection %COLLECTION_NAME% on %EXIST_URL% recreated

set USERNAME=
set PASSWORD=

goto end


:error
set USERNAME=
set PASSWORD=

echo Failed with error code %errorlevel%
exit /b %errorlevel%


:end
