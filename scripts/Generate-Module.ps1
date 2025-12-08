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
$appLines += "        // Assembly assembly = typeof(DependencyInjection).Assembly;"
$appLines += "        //"
$appLines += "        // services.AddCommandHandlers(assembly);"
$appLines += "        // services.AddQueryHandlers(assembly);"
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
$presentationLines += "using System.Reflection;"
$presentationLines += "using Core.Presentation.Endpoints;"
$presentationLines += "using Core.Presentation.Extensions;"
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
$presentationLines += "        // TODO: Add endpoint here"
$presentationLines += "        // services.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());"
$presentationLines += ""
$presentationLines += "        return services;"
$presentationLines += "    }"
$presentationLines += ""
$presentationLines += "    public static IEndpointRouteBuilder Configure${ModuleName}Presentation(this IEndpointRouteBuilder builder)"
$presentationLines += "    {"
$presentationLines += "        // TODO: Configure your route group and tags"
$presentationLines += "        // RouteGroupBuilder group = builder"
$presentationLines += "        //     .MapGroup(ApiRoutes.${ModuleName}.Base)"
$presentationLines += "        //     .WithTags(EndpointTag.${ModuleName});"
$presentationLines += ""
$presentationLines += "        // builder.MapEndpointsFromAssembly(Assembly.GetExecutingAssembly(), group);"
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

# Create csproj with copy configuration files
$facadeCsprojLines = @()
$facadeCsprojLines += '<Project Sdk="Microsoft.NET.Sdk">'
$facadeCsprojLines += ''
$facadeCsprojLines += '  <ItemGroup>'
$facadeCsprojLines += '    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" />'
$facadeCsprojLines += '    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />'
$facadeCsprojLines += '    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" />'
$facadeCsprojLines += '  </ItemGroup>'
$facadeCsprojLines += ''
$facadeCsprojLines += '  <ItemGroup>'
$facadeCsprojLines += "    <ProjectReference Include=`"..\..\Core\Core.Facade\Core.Facade.csproj`" />"
$facadeCsprojLines += "    <ProjectReference Include=`"..\$ModuleName.Domain\$ModuleName.Domain.csproj`" />"
$facadeCsprojLines += "    <ProjectReference Include=`"..\$ModuleName.Application\$ModuleName.Application.csproj`" />"
$facadeCsprojLines += "    <ProjectReference Include=`"..\$ModuleName.Persistence\$ModuleName.Persistence.csproj`" />"
$facadeCsprojLines += "    <ProjectReference Include=`"..\$ModuleName.Infrastructure\$ModuleName.Infrastructure.csproj`" />"
$facadeCsprojLines += "    <ProjectReference Include=`"..\$ModuleName.Presentation\$ModuleName.Presentation.csproj`" />"
$facadeCsprojLines += '  </ItemGroup>'
$facadeCsprojLines += ''
$facadeCsprojLines += '  <ItemGroup>'
$facadeCsprojLines += '    <None Update="*.configuration*.json">'
$facadeCsprojLines += '      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>'
$facadeCsprojLines += '    </None>'
$facadeCsprojLines += '  </ItemGroup>'
$facadeCsprojLines += ''
$facadeCsprojLines += '</Project>'

$facadeCsprojLines | Out-File -FilePath "$facadePath\$ModuleName.Facade.csproj" -Encoding UTF8

# Create Module class implementing IModule
$moduleClassLines = @()
$moduleClassLines += "using Core.Facade.Abstractions;"
$moduleClassLines += "using Core.Facade.Extensions;"
$moduleClassLines += "using Microsoft.AspNetCore.Builder;"
$moduleClassLines += "using $ModuleName.Application;"
$moduleClassLines += "using $ModuleName.Domain;"
$moduleClassLines += "using $ModuleName.Infrastructure;"
$moduleClassLines += "using $ModuleName.Persistence;"
$moduleClassLines += "using $ModuleName.Presentation;"
$moduleClassLines += ""
$moduleClassLines += "namespace $ModuleName.Facade;"
$moduleClassLines += ""
$moduleClassLines += "public sealed class ${ModuleName}Module : IModule"
$moduleClassLines += "{"
$moduleClassLines += "    public string Name => `"$ModuleName`";"
$moduleClassLines += "    public int Order => 10;"
$moduleClassLines += ""
$moduleClassLines += "    public void RegisterServices(WebApplicationBuilder builder)"
$moduleClassLines += "    {"
$moduleClassLines += "        builder.Services"
$moduleClassLines += "            .Add${ModuleName}Domain()"
$moduleClassLines += "            .Add${ModuleName}Application()"
$moduleClassLines += "            .Add${ModuleName}Persistence(builder.Configuration)"
$moduleClassLines += "            .Add${ModuleName}Infrastructure(builder.Environment, builder.Configuration)"
$moduleClassLines += "            .Add${ModuleName}Presentation();"
$moduleClassLines += "    }"
$moduleClassLines += ""
$moduleClassLines += "    public void ConfigureApplication(WebApplication app)"
$moduleClassLines += "    {"
$moduleClassLines += "        app.Configure${ModuleName}Presentation();"
$moduleClassLines += "    }"
$moduleClassLines += "}"

$moduleClassLines | Out-File -FilePath "$facadePath\${ModuleName}Module.cs" -Encoding UTF8

# Create empty configuration files
$moduleNameLower = $ModuleName.ToLower()
$configLines = @()
$configLines += "{"
$configLines += "}"

$configLines | Out-File -FilePath "$facadePath\${moduleNameLower}.configuration.json" -Encoding UTF8
$configLines | Out-File -FilePath "$facadePath\${moduleNameLower}.configuration.Development.json" -Encoding UTF8

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
Write-Host "2. In ModuleRegistry.cs add:" -ForegroundColor White
Write-Host "   using $ModuleName.Facade;" -ForegroundColor Gray
Write-Host "   public static IModule[] Modules { get; } = [new CoreModule(), new UsersModule(), new ${ModuleName}Module()];" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Configure endpoints in $ModuleName.Presentation/DependencyInjection.cs:" -ForegroundColor White
Write-Host "   - Uncomment and configure the RouteGroupBuilder" -ForegroundColor Gray
Write-Host "   - Endpoints implementing IEndpoint will be automatically registered!" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Configure module-specific settings in:" -ForegroundColor White
Write-Host "   - ${moduleNameLower}.configuration.json (production settings)" -ForegroundColor Gray
Write-Host "   - ${moduleNameLower}.configuration.Development.json (development settings)" -ForegroundColor Gray
Write-Host ""
