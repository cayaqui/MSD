@echo off
echo Starting EzPro Development Environment...
echo.

echo Starting API...
start "EzPro API" cmd /k "cd Api && dotnet run --launch-profile https"

timeout /t 5 /nobreak > nul

echo Starting Web Application...
start "EzPro Web" cmd /k "cd Web && dotnet run --launch-profile https"

echo.
echo Development environment started!
echo API: https://localhost:7193
echo Web: https://localhost:7284
echo Swagger: https://localhost:7193/swagger
echo.
pause