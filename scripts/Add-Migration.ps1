# Add-Migration.ps1
# Script to create EF Core migrations for a specific module
#
# Usage:
#   .\scripts\Add-Migration.ps1 -ModuleName Users -MigrationName InitialCreate
#   .\scripts\Add-Migration.ps1 Users InitialCreate

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$ModuleName,
    
    [Parameter(Mandatory=$true, Position=1)]
    [string]$MigrationName
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

Write-Host "Creating migration '$MigrationName' for module '$ModuleName'..." -ForegroundColor Cyan

# Run the migration command
dotnet ef migrations add $MigrationName `
    --project $persistenceProjectPath `
    --startup-project $hostProjectPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nMigration created successfully!" -ForegroundColor Green
} else {
    Write-Error "`nFailed to create migration. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}
