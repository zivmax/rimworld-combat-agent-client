@echo off

:: Remove unnecessary assemblies
del /S /Q "1.5\Assemblies\*.*"

:: Build dll
dotnet build .vscode\win

:: Create directory for managed assemblies
if not exist "..\..\RimWorldWin64_Data\Managed\AddedHistory" (
    mkdir "..\..\RimWorldWin64_Data\Managed\AddedHistory"
)

:: Find and copy assemblies, excluding Combat.Agent.dll
for /r "1.5\Assemblies" %%f in (*.dll) do (
    if not "%%~nxf" == "Combat.Agent.dll" (
        copy "%%f" "..\..\RimWorldWin64_Data\Managed\AddedHistory"
    )
)


:: Copy all files from AddedHistory to Managed folder
xcopy /Y /S "..\..\RimWorldWin64_Data\Managed\AddedHistory" "..\..\RimWorldWin64_Data\Managed"