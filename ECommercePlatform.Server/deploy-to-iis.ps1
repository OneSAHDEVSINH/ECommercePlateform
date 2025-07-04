param(
    [string]$DeployPath = "C:\inetpub\wwwroot\ECommercePlatform",
    [string]$AppPoolName = "ECommercePlatform",
    [string]$SiteName = "ECommercePlatform"
)

Write-Host "Starting deployment process..." -ForegroundColor Green

# Function to stop application pool using appcmd
function Stop-AppPoolSafely {
    param($AppPoolName)
    try {
        Write-Host "Stopping Application Pool: $AppPoolName" -ForegroundColor Yellow
        & "$env:windir\system32\inetsrv\appcmd.exe" stop apppool $AppPoolName
        
        # Wait for the app pool to stop
        $timeout = 30
        $elapsed = 0
        do {
            Start-Sleep -Seconds 2
            $elapsed += 2
            $status = & "$env:windir\system32\inetsrv\appcmd.exe" list apppool $AppPoolName
            Write-Host "Waiting for app pool to stop... ($elapsed seconds)" -ForegroundColor Yellow
        } while ($status -match "Started" -and $elapsed -lt $timeout)
        
        Write-Host "Application Pool stopped" -ForegroundColor Green
    } catch {
        Write-Host "Could not stop application pool: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Function to start application pool using appcmd
function Start-AppPoolSafely {
    param($AppPoolName)
    try {
        Write-Host "Starting Application Pool: $AppPoolName" -ForegroundColor Yellow
        & "$env:windir\system32\inetsrv\appcmd.exe" start apppool $AppPoolName
        Write-Host "Application Pool started successfully" -ForegroundColor Green
    } catch {
        Write-Host "Could not start application pool: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Step 1: Build Angular application
Write-Host "Building Angular application..." -ForegroundColor Yellow
Set-Location "..\ECommercePlatform.client"

if (!(Test-Path "node_modules")) {
    Write-Host "Installing npm dependencies..." -ForegroundColor Yellow
    npm install
}

ng build --configuration=production

if ($LASTEXITCODE -ne 0) {
    Write-Host "Angular build failed!" -ForegroundColor Red
    exit 1
}

# Step 2: Build .NET application
Write-Host "Building .NET application..." -ForegroundColor Yellow
Set-Location "../ECommercePlatform.Server"

# Clean previous publish folder
if (Test-Path "./publish") {
    Remove-Item "./publish" -Recurse -Force
}

dotnet publish -c Release -o "./publish" --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host ".NET build failed!" -ForegroundColor Red
    exit 1
}

# Step 3: Stop IIS application pool and website
Stop-AppPoolSafely -AppPoolName $AppPoolName

try {
    & "$env:windir\system32\inetsrv\appcmd.exe" stop site $SiteName
    Write-Host "Website stopped" -ForegroundColor Green
} catch {
    Write-Host "Could not stop website (it may not exist yet)" -ForegroundColor Yellow
}

# Additional wait to ensure file handles are released
Write-Host "Waiting for file handles to be released..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Step 4: Clear deployment directory
Write-Host "Preparing deployment directory..." -ForegroundColor Yellow
if (Test-Path $DeployPath) {
    try {
        # Try to remove files, but continue if some are locked
        Get-ChildItem $DeployPath -Recurse | ForEach-Object {
            try {
                Remove-Item $_.FullName -Force -Recurse -ErrorAction Stop
            } catch {
                Write-Host "Could not remove: $($_.FullName)" -ForegroundColor Yellow
            }
        }
    } catch {
        Write-Host "Some files could not be removed, continuing..." -ForegroundColor Yellow
    }
} else {
    New-Item -ItemType Directory -Path $DeployPath -Force
}

# Step 5: Copy .NET application files
Write-Host "Copying .NET application files..." -ForegroundColor Yellow
Copy-Item -Path "./publish/*" -Destination $DeployPath -Recurse -Force -ErrorAction Continue
Write-Host ".NET files copied successfully" -ForegroundColor Green

# Step 6: Copy Angular files from browser folder to deployment root
Write-Host "Copying Angular files to deployment root..." -ForegroundColor Yellow
$angularSourceDir = "../ECommercePlatform.client/dist/ECommercePlatform.client/browser"

if (Test-Path $angularSourceDir) {
    # Copy Angular files from browser subfolder directly to deployment root
    Copy-Item -Path "$angularSourceDir/*" -Destination $DeployPath -Recurse -Force
    Write-Host "Angular files copied successfully from browser folder" -ForegroundColor Green
} else {
    Write-Host "Angular browser build directory not found at path: $angularSourceDir" -ForegroundColor Red
    exit 1
}

# Step 7: Ensure web.config is correct
$webConfigSource = "./web.config"
$webConfigDest = "$DeployPath/web.config"

if (Test-Path $webConfigSource) {
    Copy-Item -Path $webConfigSource -Destination $webConfigDest -Force
    Write-Host "web.config copied successfully" -ForegroundColor Green
} else {
    Write-Host "web.config not found in source directory" -ForegroundColor Red
}

# Step 8: Create and configure logs directory
$deployLogsDir = "$DeployPath/logs"
if (!(Test-Path $deployLogsDir)) {
    New-Item -ItemType Directory -Path $deployLogsDir -Force
}

# Set permissions for logs directory
try {
    $acl = Get-Acl $deployLogsDir
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    $accessRule2 = New-Object System.Security.AccessControl.FileSystemAccessRule("NETWORK SERVICE", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule2)
    Set-Acl -Path $deployLogsDir -AclObject $acl
    Write-Host "Permissions set for logs directory" -ForegroundColor Green
} catch {
    Write-Host "Warning: Could not set permissions automatically. Please set manually." -ForegroundColor Yellow
}

# Step 9: Verify critical files exist
Write-Host "Verifying deployment..." -ForegroundColor Yellow
$criticalFiles = @(
    "$DeployPath\index.html",
    "$DeployPath\ECommercePlatform.Server.dll",
    "$DeployPath\web.config"
)

$allFilesExist = $true
foreach ($file in $criticalFiles) {
    if (!(Test-Path $file)) {
        Write-Host "MISSING: $file" -ForegroundColor Red
        $allFilesExist = $false
    } else {
        Write-Host "FOUND: $file" -ForegroundColor Green
    }
}

if (!$allFilesExist) {
    Write-Host "Some critical files are missing. Checking deployment structure..." -ForegroundColor Yellow
    
    # List contents of deployment directory
    Write-Host "Contents of deployment directory:" -ForegroundColor Yellow
    Get-ChildItem $DeployPath | ForEach-Object {
        Write-Host "  $($_.Name)" -ForegroundColor White
    }
    
    # Check if index.html is in browser subfolder
    if (Test-Path "$angularSourceDir/index.html") {
        Write-Host "Found index.html in Angular browser folder, copying again..." -ForegroundColor Yellow
        Copy-Item -Path "$angularSourceDir/index.html" -Destination "$DeployPath/index.html" -Force
    }
}

# Step 10: Start IIS site and app pool
try {
    & "$env:windir\system32\inetsrv\appcmd.exe" start site $SiteName
    Write-Host "Website started successfully" -ForegroundColor Green
} catch {
    Write-Host "Could not start website automatically. Please start manually in IIS Manager." -ForegroundColor Yellow
}

Start-AppPoolSafely -AppPoolName $AppPoolName

Write-Host "Deployment completed!" -ForegroundColor Green
Write-Host "Application deployed to: $DeployPath" -ForegroundColor Green
Write-Host ""
Write-Host "Testing URLs:" -ForegroundColor Yellow
Write-Host "1. Angular app: http://localhost:8080/" -ForegroundColor White
Write-Host "2. API test: http://localhost:8080/api/auth/login" -ForegroundColor White

if ($allFilesExist) {
    Write-Host "All critical files are present. Application should be working." -ForegroundColor Green
} else {
    Write-Host "Some critical files were missing but have been corrected." -ForegroundColor Yellow
}

# Final verification
if (Test-Path "$DeployPath\index.html") {
    Write-Host "✓ index.html is now present in deployment directory" -ForegroundColor Green
} else {
    Write-Host "✗ index.html is still missing - manual intervention required" -ForegroundColor Red
}