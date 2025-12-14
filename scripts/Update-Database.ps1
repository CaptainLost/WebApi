# Update-Database.ps1
# Script to apply EF Core migrations for a specific module
#
# Usage:
#   .\scripts\Update-Database.ps1 -ModuleName Users
#   .\scripts\Update-Database.ps1 Users
#   .\scripts\Update-Database.ps1 Users -MigrationName SpecificMigration

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$ModuleName,
    
    [Parameter(Mandatory=$false, Position=1)]
    [string]$MigrationName = ""
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

if ($MigrationName) {
    Write-Host "Updating database to migration '$MigrationName' for module '$ModuleName'..." -ForegroundColor Cyan
    dotnet ef database update $MigrationName `
        --project $persistenceProjectPath `
        --startup-project $hostProjectPath `
        --verbose
} else {
    Write-Host "Updating database to latest migration for module '$ModuleName'..." -ForegroundColor Cyan
    dotnet ef database update `
        --project $persistenceProjectPath `
        --startup-project $hostProjectPath `
        --verbose
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nDatabase updated successfully!" -ForegroundColor Green
} else {
    Write-Error "`nFailed to update database. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}
