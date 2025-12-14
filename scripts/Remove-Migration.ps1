# Remove-Migration.ps1
# Script to remove the last EF Core migration for a specific module
#
# Usage:
#   .\scripts\Remove-Migration.ps1 -ModuleName Users
#   .\scripts\Remove-Migration.ps1 Users

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$ModuleName
)

# Get the root directory of the solution
$rootDir = $PSScriptRoot | Split-Path -Parent

# Define paths
$persistenceProjectPath = Join-Path $rootDir "src\Modules\$ModuleName\$ModuleName.Persistence\$ModuleName.Persistence.csproj"
$hostProjectPath = Join-Path $rootDir "src\Host\Host.csproj"

# Validate that the persistence project exists
if (-not (Test-Path $persistenceProjectPath)) {
    Write-Error "Persistence project not found at: $persistenceProjectPath"
    Write-Error "Make sure the module name is correct."
    exit 1
}

# Validate that the host project exists
if (-not (Test-Path $hostProjectPath)) {
    Write-Error "Host project not found at: $hostProjectPath"
    exit 1
}

Write-Host "Removing last migration for module '$ModuleName'..." -ForegroundColor Cyan

# Run the remove migration command
dotnet ef migrations remove `
    --project $persistenceProjectPath `
    --startup-project $hostProjectPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nMigration removed successfully!" -ForegroundColor Green
} else {
    Write-Error "`nFailed to remove migration. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}
