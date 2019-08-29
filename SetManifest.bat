@echo off
pushd %~dp0

set proj=CLTools

Manifest\bin\Debug\Manifest.exe "%proj%\bin\Debug\%proj%.dll" "%proj%\Cmdlet" "%proj%\bin\Debug\%proj%.psd1" "%proj%\bin\Debug\%proj%.psm1"
Manifest\bin\Release\Manifest.exe "%proj%\bin\Release\%proj%.dll" "%proj%\Cmdlet" "%proj%\bin\Release\%proj%.psd1" "%proj%\bin\Release\%proj%.psm1"

pause
