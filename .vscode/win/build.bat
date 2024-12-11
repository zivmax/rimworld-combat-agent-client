echo off

:: Remove unnecessary assemblies
DEL .\*\Assemblies\*.*

:: Build dll
dotnet build .vscode\win


@echo off
:: Create directory for managed assemblies
if not exist "..\..\RimWorldWin64_Data\Managed\AddedHistory" (
    mkdir "..\..\RimWorldWin64_Data\Managed\AddedHistory"
)

:: Find and copy assemblies, excluding Combat.Agent.dll
for /r %%i in (.\*\Assemblies\*.dll) do (
    if not "%%~nxi"=="Combat.Agent.dll" (
        copy "%%i" "..\..\RimWorldWin64_Data\Managed\AddedHistory\"
    )
)

:: Copy all files from AddedHistory to Managed folder
xcopy /y "..\..\RimWorldWin64_Data\Managed\AddedHistory\*" "..\..\RimWorldWin64_Data\Managed\"