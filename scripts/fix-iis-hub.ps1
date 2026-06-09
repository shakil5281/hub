#Requires -RunAsAdministrator
# Quick fix for hub IIS site: web.config + permissions + recycle
# Usage: .\fix-iis-hub.ps1

$HubDir  = "C:\inetpub\wwwroot\hub"
$AppPool = "hub"
$SourceWebConfig = "g:\softwer\hrhub\ERP.Web\web.config"

Write-Host "=== ERP Hub IIS Fix ===" -ForegroundColor Cyan
Write-Host "App pool: $AppPool" -ForegroundColor Cyan

Write-Host "Applying web.config..." -ForegroundColor Yellow
Copy-Item $SourceWebConfig "$HubDir\web.config" -Force

Write-Host "Creating logs folder..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "$HubDir\logs" | Out-Null

Write-Host "Setting folder permissions..." -ForegroundColor Yellow
icacls $HubDir /grant "IIS AppPool\${AppPool}:(OI)(CI)RX" /T | Out-Null
icacls "$HubDir\logs" /grant "IIS AppPool\${AppPool}:(OI)(CI)F" /T | Out-Null
icacls $HubDir /grant "SYSTEM:(OI)(CI)RX" /T | Out-Null
icacls "$HubDir\logs" /grant "SYSTEM:(OI)(CI)F" /T | Out-Null

Write-Host "Recycling app pool '$AppPool' ..." -ForegroundColor Yellow
Import-Module WebAdministration -ErrorAction SilentlyContinue
if (Get-Module WebAdministration) {
    Restart-WebAppPool -Name $AppPool
} else {
    iisreset /restart | Out-Null
}

Write-Host ""
Write-Host "IIS config updated." -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT - if you still get 502.5, run this in SSMS:" -ForegroundColor Yellow
Write-Host "  g:\softwer\hrhub\scripts\setup-erphub-database.sql"
Write-Host ""
Write-Host "The stdout log showed: CREATE DATABASE permission denied."
Write-Host "The database must exist and IIS identity needs db_owner access."
Write-Host ""
Write-Host "Test: http://localhost/hub/Account/Login"
Write-Host "Log:  $HubDir\logs\stdout_*.log"
