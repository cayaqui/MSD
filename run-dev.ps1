# Script para ejecutar API y Web en desarrollo

Write-Host "Starting EzPro Development Environment..." -ForegroundColor Green

# Iniciar la API en una nueva ventana
Write-Host "Starting API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Api; dotnet run --launch-profile https"

# Esperar 5 segundos para que la API se inicie
Start-Sleep -Seconds 5

# Iniciar la aplicación Web en una nueva ventana
Write-Host "Starting Web Application..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Web; dotnet run --launch-profile https"

Write-Host "Development environment started!" -ForegroundColor Green
Write-Host "API: https://localhost:7193" -ForegroundColor Cyan
Write-Host "Web: https://localhost:7284" -ForegroundColor Cyan
Write-Host "Swagger: https://localhost:7193/swagger" -ForegroundColor Cyan