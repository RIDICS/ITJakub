@echo off
setlocal EnableDelayedExpansion

SET PARAM=%1
SET OK=1

REM No OR no fun...
if [%PARAM%] EQU [] SET OK=0
if %PARAM% == help SET OK=0
if %PARAM% == /help SET OK=0
if %PARAM% == /h SET OK=0
if %PARAM% == --help SET OK=0
if %PARAM% == -h SET OK=0

if %OK% EQU 0 (
	echo "Please check default paths in script, then run me with any parameter"
	pause 60
	exit /B
)

SET CURRENTDIR=D:\Data\Raw_books\Slovniky

SET DIR_SOURCE_DOCX=%CURRENTDIR%\\ESSC
SET DIR_NUMBERED_SOURCE_DOCX=%CURRENTDIR%\\ESSC_TMP
SET DIR_OUTPUT=%CURRENTDIR%\\ESSC.zip

SET ZIP="C:\Program Files\7-Zip\7z.exe"

mkdir "%DIR_NUMBERED_SOURCE_DOCX%"

for %%f in ("%DIR_SOURCE_DOCX%\\*.*") do (
	SET /A "COUNTER+=1"

	SET SOURCE_STRING=%%~nf
	SET REPLACETEXT=_!COUNTER!_
	CALL SET NEW_NAME=%%SOURCE_STRING:_=!REPLACETEXT!%%

	copy "%%f" "%DIR_NUMBERED_SOURCE_DOCX%\\!NEW_NAME!%%~xf"
)

for %%f in ("%DIR_NUMBERED_SOURCE_DOCX%\\*.*") do (
	%ZIP% a "%DIR_OUTPUT%" "%%f"
)
