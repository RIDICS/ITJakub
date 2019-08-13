@ECHO OFF

SET PROJ_DIR=%~dp0
echo Using project directory: %PROJ_DIR%

RD /S /Q "%PROJ_DIR%bin\Migrator-build"

dotnet publish "%PROJ_DIR%Vokabular.Database.Migrator.csproj" --configuration Release --output "%PROJ_DIR%/bin/Migrator-build/"
