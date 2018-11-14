@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
REM This is a script used to generate assembly information, which is executed when the AssemblyInfoGenerator project is generated
REM Update the version number according to the T4 template carried in the project.
REM RevisionNumber = ((int)(DateTime.UtcNow - new DateTime(2017,1,1)).TotalDays)*10+((int)DateTime.UtcNow.Hour)/3
:: set the working dir (default to current dir)
set wdir=%cd%
if not (%1)==() set wdir=%1

:: set the file extension (default to vb)
set extension=cs
if not (%2)==() set extension=%2

echo executing transform_all from %wdir%
pushd %wdir%
:: create a list of all the T4 templates in the working dir
dir *.tt /b > t4list.txt

echo the following T4 templates will be transformed:
type t4list.txt

set TextTransform=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\TextTransform.exe

:: transform all the templates
for /f %%d in (t4list.txt) do (
set file_name=%%d
set file_name=!file_name:~0,-3!.%extension%
echo:  \--^> !file_name!    
"%TextTransform%" -out !file_name! %%d
)

:: delete T4 list and return to previous directory
del t4list.txt
popd

echo transformation complete
