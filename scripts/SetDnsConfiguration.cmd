@echo off
pushd %~dp0
for %%a in ("%PSModulePath:;=";"%") do if %%a=="%~dp0" goto Exec
set PSModulePath=%PsModulePath%;%~dp0
:Exec
powershell.exe -ExecutionPolicy Unrestricted .\%~nn0.ps1 %*
popd