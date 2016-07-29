@echo off

call config.bat

devenv /build %config% SharpECS.sln

cls
echo  Build done!