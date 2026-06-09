#Requires -RunAsAdministrator
# Build and publish ERP Hub to C:\inetpub\wwwroot\hub
# Usage: Right-click PowerShell -> Run as Administrator, then:
#   cd g:\softwer\hrhub\scripts
#   .\deploy-to-iis.ps1

$ErrorActionPreference = "Stop"

$ProjectDir = "g:\softwer\hrhub\ERP.Web"
$StagingDir = "g:\softwer\hrhub\publish-staging"
$IisDir     = "C:\inetpub\wwwroot\hub"
$AppPool    = "hub"

Write-Host "=== ERP Hub IIS Deploy ===" -ForegroundColor Cyan

# 1. Stop IIS to release file locks
Write-Host "Stopping IIS..." -ForegroundColor Yellow
iisreset /stop | Out-Null
Start-Sleep -Seconds 2

# 2. Build Tailwind CSS
Write-Host "Building CSS..." -ForegroundColor Yellow
Push-Location $ProjectDir
npm run build:css
if ($LASTEXITCODE -ne 0) { throw "CSS build failed" }

# 3. Publish to staging
Write-Host "Publishing Release build..." -ForegroundColor Yellow
dotnet publish -c Release -o $StagingDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
Pop-Location

# 4. Copy to IIS folder
Write-Host "Copying to $IisDir ..." -ForegroundColor Yellow
if (-not (Test-Path $IisDir)) {
    New-Item -ItemType Directory -Force -Path $IisDir | Out-Null
}
robocopy $StagingDir $IisDir /MIR /R:2 /W:2 /NFL /NDL /NJH /NJS | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed with code $LASTEXITCODE" }

# 5. Create logs folder for stdout logging
New-Item -ItemType Directory -Force -Path "$IisDir\logs" | Out-Null

# 6. Grant IIS App Pool permissions
Write-Host "Setting folder permissions for IIS AppPool\$AppPool ..." -ForegroundColor Yellow
icacls $IisDir /grant "IIS AppPool\${AppPool}:(OI)(CI)RX" /T | Out-Null
icacls "$IisDir\logs" /grant "IIS AppPool\${AppPool}:(OI)(CI)F" /T | Out-Null

# 7. Start IIS
Write-Host "Starting IIS..." -ForegroundColor Yellow
iisreset /start | Out-Null

Write-Host ""
Write-Host "Deploy complete!" -ForegroundColor Green
Write-Host "URL: http://localhost/hub/Account/Login"
Write-Host "Login: admin@erp.com / Admin@123"
Write-Host ""
Write-Host "If errors occur, check: $IisDir\logs\stdout_*.log"
Write-Host "Run setup-erphub-database.sql in SSMS if SQL permission errors appear."
Write-Host ""
Write-Host "Note: hostingModel is outofprocess to match sibling apps under Default Web Site."
