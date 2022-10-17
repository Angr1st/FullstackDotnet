Param(
    [string]$DbConnectionString = "Server=(localdb)\mssqllocaldb;Database=SimpleDatabase"
)

if (!$DbConnectionString)
{
    Write-Host "You need to supply a DbConnectionString"
    return
}

#Download SqlPackage.Exe
$extractFolderName = "./SqlPackage"

#Delete Folder if it exists
if (Test-Path $extractFolderName) {
    Remove-Item $extractFolderName -Recurse -Force
}

mkdir $extractFolderName

$url = "https://go.microsoft.com/fwlink/?linkid=2209609"

$sqlPackageZip = "./sqlpackage.zip"

Write-Host "Download SqlPackage.exe from microsoft."
$oldProgressPreference = $global:progressPreference;
$global:progressPreference = 'silentlyContinue'

Invoke-WebRequest -Uri $url -OutFile $sqlPackageZip

# unzip
Expand-Archive -Path  $sqlPackageZip -DestinationPath $extractFolderName

$global:progressPreference = $oldProgressPreference

Write-Host "Publish binaries in release mode"

dotnet publish -c Release ./Server/Server.csproj

#https://stackoverflow.com/a/66317874/6225469
Function Hide-BlazorDLL {

    Param(
        [string]$Path = (Get-Location).Path
    )

    <#
        According to the following Links:
        https://github.com/dotnet/aspnetcore/issues/19552
        https://github.com/dotnet/aspnetcore/issues/5477#issuecomment-599148931
        https://github.com/dotnet/aspnetcore/issues/21489
        https://gist.github.com/Swimburger/774ca2b63bad4a16eb2fa23b47297e71 
    #>

    # Test if path is correct and accessible
    $WorkingDir = Join-Path $Path "_framework"
    if (!(Test-Path $WorkingDir)) { Throw "Wrong path $Path. current location must be wwwroot folder of published application." }

    # Get All Items
    $AllItems = Get-ChildItem $WorkingDir -Recurse
    $DLLs = $AllItems | Where-Object { $_.Name -like '*.dll*' }
    $BINs = $AllItems | Where-Object { $_.Name -like '*.bin*' }

    # End script if no .dll are found
    if ($DLLs) { 

        # Delete all current .bin files
        if ($BINs) {
            Remove-item $BINs.FullName -Force
        }
    
        # Change .dll to .bin on files and config
        $DLLs | Rename-item -NewName { $_.Name -replace ".dll\b",".bin" }
        ((Get-Content "$WorkingDir\blazor.boot.json" -Raw) -replace '.dll"','.bin"') | Set-Content "$WorkingDir\blazor.boot.json"
    
        # Delete Compressed Blazor files
        if (Test-Path "$WorkingDir\blazor.boot.json.gz") {
            Remove-Item "$WorkingDir\blazor.boot.json.gz"
        }
        if (Test-Path "$WorkingDir\blazor.boot.json.br") {
            Remove-Item "$WorkingDir\blazor.boot.json.br"
        }
    
        # Do the same for ServiceWorker, if it exists
        $ServiceWorker = Get-Item "$Path\service-worker-assets.js" -ErrorAction SilentlyContinue
        if ($ServiceWorker) {
            ((Get-Content $ServiceWorker.FullName -Raw) -replace '.dll"','.bin"') | Set-Content $ServiceWorker.FullName
            Remove-Item ($ServiceWorker.FullName + ".gz")
            Remove-Item ($ServiceWorker.FullName + ".br")
        }
    }
    else {
        Write-Host "There are no .dll Files to rename to .bin" 
    }
}

Write-Host "Hide all blazor dll files (by turning them into bin files)"

Hide-BlazorDLL -Path ./Server/bin/Release/net7.0/publish/wwwroot/

Write-Host "Deploy the .dacpac for the Database"

Set-Location -Path ./SimpleDatabase.Sdk

dotnet clean /p:NetCoreBuild=true

dotnet build -c Release /p:NetCoreBuild=true

Set-Location -Path ..

$sqlPackageExe = $extractFolderName + "/sqlpackage.exe"

$targetConnectionString = "/TargetConnectionString:" + $DbConnectionString

$sqlProcess = Start-Process -NoNewWindow -PassThru $sqlPackageExe -ArgumentList "/Action:Publish", "/SourceFile:./SimpleDatabase.Sdk/bin/Release/SimpleDatabase.Sdk.dacpac", $targetConnectionString

$sqlProcess.WaitForExit()

Write-Host "Start Webserver"

$webServerProcess = Start-Process -NoNewWindow -PassThru ./Server/bin/Release/net7.0/publish/Server.exe -wo ./Server/bin/Release/net7.0/publish/ -ArgumentList "--dbconnectionstring", $DbConnectionString

$webServerProcess.WaitForExit()

Write-Host "Exiting the script!"