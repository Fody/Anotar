$ErrorActionPreference = 'Stop'

# TUnit test projects are executables. They are run with 'dotnet run' for each target framework.
$projects = @(
    'Catel\Tests\Tests.csproj',
    'CommonLogging\Tests\Tests.csproj',
    'Custom\Tests\Tests.csproj',
    'NLog\Tests\Tests.csproj',
    'NServiceBus\Tests\Tests.csproj',
    'Serilog\Tests\Tests.csproj',
    'Splat\Tests\Tests.csproj'
)

$failed = @()
foreach ($project in $projects) {
    $frameworks = (dotnet msbuild $project -getProperty:TargetFrameworks -p:Configuration=Release).Trim().Split(';')
    if (-not $frameworks[0]) {
        $frameworks = @((dotnet msbuild $project -getProperty:TargetFramework -p:Configuration=Release).Trim())
    }
    foreach ($framework in $frameworks) {
        Write-Host "Running $project ($framework)"
        dotnet run --project $project -c Release -f $framework --no-build
        if ($LASTEXITCODE -ne 0) {
            $failed += "$project ($framework)"
        }
    }
}

if ($failed.Count -gt 0) {
    Write-Host "Failed test runs:`n$($failed -join "`n")"
    exit 1
}
