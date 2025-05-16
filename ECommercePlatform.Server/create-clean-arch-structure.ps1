# Create Clean Architecture Directory Structure
$directories = @(
    "src\Core\Domain\Entities",
    "src\Core\Domain\Enums",
    "src\Core\Domain\Interfaces",
    "src\Core\Application\Interfaces",
    "src\Core\Application\Services",
    "src\Core\Application\DTOs",
    "src\Infrastructure\Persistence",
    "src\Infrastructure\Persistence\Configurations",
    "src\Infrastructure\Persistence\Repositories",
    "src\Infrastructure\Services",
    "src\Presentation\Controllers",
    "src\Presentation\Middleware"
)

foreach ($dir in $directories) {
    New-Item -ItemType Directory -Path $dir -Force
    Write-Host "Created directory: $dir"
} 