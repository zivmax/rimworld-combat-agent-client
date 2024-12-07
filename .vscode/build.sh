#!/bin/bash

# Remove unnecessary assemblies
find . -type d -name 'Assemblies' -exec rm -rf {}/* \;

# Build DLL
dotnet build .vscode
