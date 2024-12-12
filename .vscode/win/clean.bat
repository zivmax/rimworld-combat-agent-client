echo off

:: Remove unnecessary assemblies
DEL .\*\Assemblies\*.*

:: If the AddedHistory folder doesn't exist, exit
if not exist "..\..\RimWorldWin64_Data\Managed\AddedHistory" (
    exit
)

:: Clean the Managed\ according to the AddedHistory folder's contents
for /r %%i in ("..\..\RimWorldWin64_Data\Managed\AddedHistory") do (
    del "..\..\RimWorldWin64_Data\Managed\%%~nxi"
)

:: Clean the AddedHistory folder
del "..\..\RimWorldWin64_Data\Managed\AddedHistory\*.*"
