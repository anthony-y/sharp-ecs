@echo off

cls

call config.bat

echo Executing!
echo(
echo ================================
echo(

cd Builds/SharpECS.Samples/%config%
SharpECS.Samples.exe

echo(
echo ================================
echo(
cd ..
echo Done!
echo(