@echo off

call config.bat

devenv /build %config% SharpECS.sln
devenv SharpECS.sln /build %config% /project "SharpECS.Samples/SharpECS.Samples.csproj" /projectconfig %config%

cls
echo  Build done!
run.bat