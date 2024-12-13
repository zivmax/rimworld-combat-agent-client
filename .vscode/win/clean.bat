@echo off

:: Remove unnecessary assemblies from mod folder
del /S /Q "1.5\Assemblies\*.*"

:: Remove assemblies from game directory
del /S /Q "..\..\RimWorldWin64_Data\Managed\AddedHistory\*.*"