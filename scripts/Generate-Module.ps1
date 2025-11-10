<#
.SYNOPSIS
    Creates a new module following Clean Architecture pattern in the WebApi project.

.PARAMETER ModuleName
    Name of the module to create (e.g., "Authentication", "Orders", "Payments")

.EXAMPLE
    .\Generate-Module-Simple.ps1 -ModuleName "Orders"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ModuleName
)

# Output colors
function Write-Step {
    param([string]$Message)
    Write-Host "-> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Constants
$SolutionFile = "..\WebApi.sln"
$ModulesBasePath = "..\src\Modules"

# Check if we are in the scripts directory
if (-not (Test-Path $SolutionFile)) {
    Write-ErrorMsg "$SolutionFile not found. Make sure you run the script from the scripts directory."
    exit 1
}

$solutionPath = Resolve-Path $SolutionFile
$modulesPath = "$ModulesBasePath\$ModuleName"

# Check if module already exists
if (Test-Path $modulesPath) {
    Write-ErrorMsg "Module '$ModuleName' already exists in $modulesPath"
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Creating module: $ModuleName" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# Function to create a simple csproj file
function New-ProjectFile {
    param(
        [string]$Path,
        [string[]]$Packages = @(),
        [string[]]$ProjectReferences = @()
    )
    
    $lines = @()
    $lines += '<Project Sdk="Microsoft.NET.Sdk">'
    $lines += ''
    
    if ($Packages.Count -gt 0) {
        $lines += '  <ItemGroup>'
        foreach ($pkg in $Packages) {
            $lines += "    <PackageReference Include=`"$pkg`" />"
        }
        $lines += '  </ItemGroup>'
        $lines += ''
    }
    
    if ($ProjectReferences.Count -gt 0) {
        $lines += '  <ItemGroup>'
        foreach ($ref in $ProjectReferences) {
            $lines += "    <ProjectReference Include=`"$ref`" />"
        }
        $lines += '  </ItemGroup>'
        $lines += ''
    }
    
    $lines += '</Project>'
    
    $lines | Out-File -FilePath $Path -Encoding UTF8
}

# Function to create DependencyInjection.cs
function New-DependencyInjectionFile {
    param(
        [string]$Path,
        [string]$Namespace,
        [string]$MethodName,
        [string[]]$Usings = @('Microsoft.Extensions.DependencyInjection'),
        [string[]]$Parameters = @()
    )
    
    $lines = @()
    
    foreach ($using in $Usings) {
        $lines += "using $using;"
    }
    $lines += ""
    $lines += "namespace $Namespace;"
    $lines += ""
    $lines += "public static class DependencyInjection"
    $lines += "{"
    
    $methodParams = "this IServiceCollection services"
    if ($Parameters.Count -gt 0) {
        $methodParams += ","
        $lines += "    public static IServiceCollection $MethodName("
        $lines += "        $methodParams"
        foreach ($param in $Parameters) {
            $lines += "        $param" + $(if ($param -ne $Parameters[-1]) { "," } else { ")" })
        }
    } else {
        $lines += "    public static IServiceCollection $MethodName($methodParams)"
    }
    
    $lines += "    {"
    $lines += "        return services;"
    $lines += "    }"
    $lines += "}"
    
    $lines | Out-File -FilePath $Path -Encoding UTF8
}

# 1. Create Domain project
Write-Step "Creating $ModuleName.Domain..."
$domainPath = "$modulesPath\$ModuleName.Domain"
New-Item -ItemType Directory -Path $domainPath -Force | Out-Null

New-ProjectFile -Path "$domainPath\$ModuleName.Domain.csproj" `
    -Packages @('Microsoft.Extensions.DependencyInjection.Abstractions') `
    -ProjectReferences @("..\..\Core\Core.Domain\Core.Domain.csproj")

New-DependencyInjectionFile `
    -Path "$domainPath\DependencyInjection.cs" `
    -Namespace "$ModuleName.Domain" `
    -MethodName "Add${ModuleName}Domain"

dotnet sln $solutionPath add "$domainPath\$ModuleName.Domain.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Domain"

# 2. Create Application project
Write-Step "Creating $ModuleName.Application..."
$appPath = "$modulesPath\$ModuleName.Application"
New-Item -ItemType Directory -Path $appPath -Force | Out-Null

New-ProjectFile -Path "$appPath\$ModuleName.Application.csproj" `
    -Packages @('Microsoft.Extensions.DependencyInjection.Abstractions') `
    -ProjectReferences @(
        "..\..\Core\Core.Application\Core.Application.csproj",
        "..\$ModuleName.Domain\$ModuleName.Domain.csproj"
    )

# Create DependencyInjection.cs with handler registration
$appLines = @()
$appLines += "using System.Reflection;"
$appLines += "using Core.Application.Extensions;"
$appLines += "using Microsoft.Extensions.DependencyInjection;"
$appLines += ""
$appLines += "namespace $ModuleName.Application;"
$appLines += ""
$appLines += "public static class DependencyInjection"
$appLines += "{"
$appLines += "    public static IServiceCollection Add${ModuleName}Application(this IServiceCollection services)"
$appLines += "    {"
$appLines += "        Assembly assembly = typeof(DependencyInjection).Assembly;"
$appLines += ""
$appLines += "        services.AddCommandHandlers(assembly);"
$appLines += "        services.AddQueryHandlers(assembly);"
$appLines += ""
$appLines += "        return services;"
$appLines += "    }"
$appLines += "}"

$appLines | Out-File -FilePath "$appPath\DependencyInjection.cs" -Encoding UTF8

dotnet sln $solutionPath add "$appPath\$ModuleName.Application.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Application"

# 3. Create Persistence project
Write-Step "Creating $ModuleName.Persistence..."
$persistencePath = "$modulesPath\$ModuleName.Persistence"
New-Item -ItemType Directory -Path $persistencePath -Force | Out-Null

New-ProjectFile -Path "$persistencePath\$ModuleName.Persistence.csproj" `
    -Packages @(
        'Microsoft.Extensions.Configuration.Abstractions',
        'Microsoft.Extensions.DependencyInjection.Abstractions'
    ) `
    -ProjectReferences @(
        "..\..\Core\Core.Persistence\Core.Persistence.csproj",
        "..\$ModuleName.Application\$ModuleName.Application.csproj"
    )

New-DependencyInjectionFile `
    -Path "$persistencePath\DependencyInjection.cs" `
    -Namespace "$ModuleName.Persistence" `
    -MethodName "Add${ModuleName}Persistence" `
    -Usings @('Microsoft.Extensions.Configuration', 'Microsoft.Extensions.DependencyInjection') `
    -Parameters @('IConfiguration configuration')

dotnet sln $solutionPath add "$persistencePath\$ModuleName.Persistence.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Persistence"

# 4. Create Infrastructure project
Write-Step "Creating $ModuleName.Infrastructure..."
$infraPath = "$modulesPath\$ModuleName.Infrastructure"
New-Item -ItemType Directory -Path $infraPath -Force | Out-Null

New-ProjectFile -Path "$infraPath\$ModuleName.Infrastructure.csproj" `
    -Packages @(
        'Microsoft.Extensions.Configuration.Abstractions',
        'Microsoft.Extensions.DependencyInjection.Abstractions',
        'Microsoft.Extensions.Hosting.Abstractions'
    ) `
    -ProjectReferences @(
        "..\..\Core\Core.Infrastructure\Core.Infrastructure.csproj",
        "..\$ModuleName.Application\$ModuleName.Application.csproj"
    )

New-DependencyInjectionFile `
    -Path "$infraPath\DependencyInjection.cs" `
    -Namespace "$ModuleName.Infrastructure" `
    -MethodName "Add${ModuleName}Infrastructure" `
    -Usings @('Microsoft.Extensions.Configuration', 'Microsoft.Extensions.DependencyInjection', 'Microsoft.Extensions.Hosting') `
    -Parameters @('IHostEnvironment environment', 'IConfiguration configuration')

dotnet sln $solutionPath add "$infraPath\$ModuleName.Infrastructure.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Infrastructure"

# 5. Create Presentation project
Write-Step "Creating $ModuleName.Presentation..."
$presentationPath = "$modulesPath\$ModuleName.Presentation"
New-Item -ItemType Directory -Path $presentationPath -Force | Out-Null

New-ProjectFile -Path "$presentationPath\$ModuleName.Presentation.csproj" `
    -Packages @('Microsoft.Extensions.DependencyInjection.Abstractions') `
    -ProjectReferences @(
        "..\..\Core\Core.Presentation\Core.Presentation.csproj",
        "..\$ModuleName.Application\$ModuleName.Application.csproj"
    )

# Create DependencyInjection.cs for Presentation with endpoint configuration
$presentationLines = @()
$presentationLines += "using Core.Presentation.Endpoints;"
$presentationLines += "using Microsoft.AspNetCore.Builder;"
$presentationLines += "using Microsoft.AspNetCore.Http;"
$presentationLines += "using Microsoft.AspNetCore.Routing;"
$presentationLines += "using Microsoft.Extensions.DependencyInjection;"
$presentationLines += ""
$presentationLines += "namespace $ModuleName.Presentation;"
$presentationLines += ""
$presentationLines += "public static class DependencyInjection"
$presentationLines += "{"
$presentationLines += "    public static IServiceCollection Add${ModuleName}Presentation(this IServiceCollection services)"
$presentationLines += "    {"
$presentationLines += "        // TODO: Register endpoints here"
$presentationLines += "        // services.AddTransient<YourEndpoint>();"
$presentationLines += ""
$presentationLines += "        return services;"
$presentationLines += "    }"
$presentationLines += ""
$presentationLines += "    public static IEndpointRouteBuilder Configure${ModuleName}Presentation(this IEndpointRouteBuilder builder)"
$presentationLines += "    {"
$presentationLines += "        // TODO: Map endpoint groups here"
$presentationLines += "        // RouteGroupBuilder group = builder"
$presentationLines += "        //     .MapGroup(ApiRoutes.YourModule.Base)"
$presentationLines += "        //     .WithTags(EndpointTag.YourModule);"
$presentationLines += ""
$presentationLines += "        // builder.ServiceProvider.GetRequiredService<YourEndpoint>().MapEndpoint(group);"
$presentationLines += ""
$presentationLines += "        return builder;"
$presentationLines += "    }"
$presentationLines += "}"

$presentationLines | Out-File -FilePath "$presentationPath\DependencyInjection.cs" -Encoding UTF8

dotnet sln $solutionPath add "$presentationPath\$ModuleName.Presentation.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Presentation"

# 6. Create Facade project
Write-Step "Creating $ModuleName.Facade..."
$facadePath = "$modulesPath\$ModuleName.Facade"
New-Item -ItemType Directory -Path $facadePath -Force | Out-Null

New-ProjectFile -Path "$facadePath\$ModuleName.Facade.csproj" `
    -Packages @(
        'Microsoft.Extensions.Configuration.Abstractions',
        'Microsoft.Extensions.DependencyInjection.Abstractions',
        'Microsoft.Extensions.Hosting.Abstractions'
    ) `
    -ProjectReferences @(
        "..\$ModuleName.Domain\$ModuleName.Domain.csproj",
        "..\$ModuleName.Application\$ModuleName.Application.csproj",
        "..\$ModuleName.Persistence\$ModuleName.Persistence.csproj",
        "..\$ModuleName.Infrastructure\$ModuleName.Infrastructure.csproj",
        "..\$ModuleName.Presentation\$ModuleName.Presentation.csproj"
    )

# Create ModuleExtensions.cs for Facade
$facadeLines = @()
$facadeLines += "using $ModuleName.Application;"
$facadeLines += "using $ModuleName.Domain;"
$facadeLines += "using $ModuleName.Infrastructure;"
$facadeLines += "using $ModuleName.Persistence;"
$facadeLines += "using $ModuleName.Presentation;"
$facadeLines += "using Microsoft.AspNetCore.Builder;"
$facadeLines += "using Microsoft.Extensions.Configuration;"
$facadeLines += "using Microsoft.Extensions.DependencyInjection;"
$facadeLines += "using Microsoft.Extensions.Hosting;"
$facadeLines += ""
$facadeLines += "namespace $ModuleName.Facade;"
$facadeLines += ""
$facadeLines += "public static class ModuleExtensions"
$facadeLines += "{"
$facadeLines += "    public static IServiceCollection Add${ModuleName}Module("
$facadeLines += "        this IServiceCollection services,"
$facadeLines += "        IHostEnvironment environment,"
$facadeLines += "        IConfiguration configuration)"
$facadeLines += "    {"
$facadeLines += "        services"
$facadeLines += "            .Add${ModuleName}Domain()"
$facadeLines += "            .Add${ModuleName}Application()"
$facadeLines += "            .Add${ModuleName}Persistence(configuration)"
$facadeLines += "            .Add${ModuleName}Infrastructure(environment, configuration)"
$facadeLines += "            .Add${ModuleName}Presentation();"
$facadeLines += ""
$facadeLines += "        return services;"
$facadeLines += "    }"
$facadeLines += ""
$facadeLines += "    public static WebApplication Configure${ModuleName}Module(this WebApplication app)"
$facadeLines += "    {"
$facadeLines += "        app.Configure${ModuleName}Presentation();"
$facadeLines += ""
$facadeLines += "        return app;"
$facadeLines += "    }"
$facadeLines += "}"

$facadeLines | Out-File -FilePath "$facadePath\ModuleExtensions.cs" -Encoding UTF8

dotnet sln $solutionPath add "$facadePath\$ModuleName.Facade.csproj" 2>&1 | Out-Null
Write-Success "Created $ModuleName.Facade"

# Build solution
Write-Host ""
Write-Step "Building solution..."
$buildResult = dotnet build $solutionPath 2>&1 | Out-String
if ($LASTEXITCODE -eq 0) {
    Write-Success "Build completed successfully!"
} else {
    Write-ErrorMsg "Build failed:"
    Write-Host $buildResult
    exit 1
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Module '$ModuleName' has been created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

Write-Host "Structure:" -ForegroundColor Cyan
Write-Host "  src/Modules/$ModuleName/" -ForegroundColor White
Write-Host "    - $ModuleName.Domain" -ForegroundColor Gray
Write-Host "    - $ModuleName.Application" -ForegroundColor Gray
Write-Host "    - $ModuleName.Persistence" -ForegroundColor Gray
Write-Host "    - $ModuleName.Infrastructure" -ForegroundColor Gray
Write-Host "    - $ModuleName.Presentation" -ForegroundColor Gray
Write-Host "    - $ModuleName.Facade" -ForegroundColor Gray
Write-Host ""

Write-Host "To integrate the module with Host:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Add reference to Host.csproj:" -ForegroundColor White
Write-Host "   <ProjectReference Include=`"..\Modules\$ModuleName\$ModuleName.Facade\$ModuleName.Facade.csproj`" />" -ForegroundColor Gray
Write-Host ""
Write-Host "2. In HostApplicationBuilderExtensions.cs add:" -ForegroundColor White
Write-Host "   using $ModuleName.Facade;" -ForegroundColor Gray
Write-Host "   .Add${ModuleName}Module(builder.Environment, builder.Configuration)" -ForegroundColor Gray
Write-Host ""
Write-Host "3. In WebApplicationExtensions.cs add:" -ForegroundColor White
Write-Host "   using $ModuleName.Facade;" -ForegroundColor Gray
Write-Host "   .Configure${ModuleName}Module()" -ForegroundColor Gray
Write-Host ""
