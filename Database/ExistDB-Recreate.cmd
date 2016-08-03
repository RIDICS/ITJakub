@echo off

set EXIST_URL=%1
set SCRIPT_PATH=%2
set COLLECTION_NAME=%3

if %1.==. set EXIST_URL=xmldb:exist://localhost:8080/exist/xmlrpc
if %2.==. set SCRIPT_PATH="C:\Pool\itjakub\Database\ExistDB"
if %3.==. set COLLECTION_NAME=jacob

set EXIST_HOME="C:\eXist-db"
set USERNAME=admin
set PASSWORD=admin


echo Recreating eXist-db for collection %COLLECTION_NAME% on %EXIST_URL%
echo --------------------

rem delete old data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -c /db/apps -R %COLLECTION_NAME%
echo --------------------

rem upload new data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH%
echo --------------------

rem delete old configuration data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -c /db/system/config/db/apps -R %COLLECTION_NAME%
echo --------------------

rem upload configuration data
java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/system/config/db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH%\config
echo --------------------

rem upload files for jacob-develop collection
if "%COLLECTION_NAME%" == "jacob-develop" java -Dexist.home=%EXIST_HOME% -jar %EXIST_HOME%\start.jar client -ouri=%EXIST_URL% -u %USERNAME% -P %PASSWORD% -d -m /db/apps/%COLLECTION_NAME% -p %SCRIPT_PATH%ForDevelopment

echo --------------------
echo Collection %COLLECTION_NAME% on %EXIST_URL% recreated

set USERNAME=
set PASSWORD=
