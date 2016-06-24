@echo off
Setlocal 

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

SET CURRENTDIR=D:\Data\Raw_books

SET DIR_DOCX=%CURRENTDIR%\\Mluvnice_Docx
SET DIR_IMAGES=%CURRENTDIR%\\Mluvnice
SET DIR_XML=%CURRENTDIR%\\Mluvnice
SET DIR_COMBINED=%CURRENTDIR%\\Mluvnice_combined

SET ZIP="C:\Program Files\7-Zip\7z.exe"


for %%f in ("%DIR_DOCX%\\*.*") do (
	if %%~nf NEQ aaa_identifikatory_mluvnic (
		%ZIP% a "%DIR_COMBINED%\\%%~nf.zip" "%%f" "%DIR_XML%\\%%~nf.xml" "%DIR_IMAGES%\\%%~nf"
	
		exit /B
	)
)
