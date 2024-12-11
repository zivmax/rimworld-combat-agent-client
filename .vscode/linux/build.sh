#!/bin/bash

# Remove unnecessary assemblies
rm -rf ./*/Assemblies/*.*

# Build DLL
dotnet build .vscode/linux

mkdir -p ../../RimWorldLinux_Data/Managed/AddedHistory

find ./*/Assemblies -type f ! -name 'Combat.Agent.dll' -exec cp {} ../../RimWorldLinux_Data/Managed/AddedHistory \;

cp ../../RimWorldLinux_Data/Managed/AddedHistory/* ../../RimWorldLinux_Data/Managed/
