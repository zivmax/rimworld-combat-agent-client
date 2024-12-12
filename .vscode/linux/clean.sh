#!/bin/bash

# Remove unnecessary assemblies
rm -f ./*/Assemblies/*.*

# If the AddedHistory folder doesn't exist, exit
if [ ! -d "../../RimWorldWin64_Data/Managed/AddedHistory" ]; then
    exit
fi

# Clean the Managed/ according to the AddedHistory folder's contents
for file in ../../RimWorldWin64_Data/Managed/AddedHistory; do
    rm -f "../../RimWorldWin64_Data/Managed/$(basename "$file")"
done

# Clean the AddedHistory folder
rm -f ../../RimWorldWin64_Data/Managed/AddedHistory/*.*
