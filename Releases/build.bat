REM Copy ../Art/icon.png to the current directory
copy /y ..\Art\icon.png .
REM Copy ../Art/manifest.json to the current directory
copy /y ..\Art\manifest.json .
REM Copy ../README.md to the current directory
copy /y ..\README.md .
REM Copy ../CHANGELOG.md to the current directory
copy /y ..\CHANGELOG.md .
REM Copy all files from ../GarageDoorFix/build/bin/Debug to the current directory
xcopy /s /y /q ..\GarageDoorFix\build\bin\Debug\* .

REM Create a zip file named GarageDoorFix.zip containing all files (except build.bat) in the current directory
"C:\Program Files\7-Zip\7z.exe" a GarageDoorFix.zip * -x!build.bat -x!GarageDoorFix.zip
