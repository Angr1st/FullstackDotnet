$ErrorActionPreference = "Stop"

function Build-Dotnet {

    Param(
        [string]$projPath,
        [string]$additionalParameter = "",
        [bool]$clean = $false
    )

    Write-Host "Entering Build-Dotnet with this projPath:" + $projPath 

    $extras = @{}
    
    $paramList = New-Object System.Collections.Generic.List[string]
    $paramList.Add("build")
    $paramList.Add("-c")
    $paramList.Add("Release")
    if ($additionalParameter) {
        $paramList.Add($additionalParameter)
    }
    $paramList.Add($projPath)
    $parameters = $paramList.ToArray()
    $extras["ArgumentList"] = $parameters       

    if ($clean)
    {
        $cleanProcess = Start-Process -NoNewWindow -PassThru -FilePath "dotnet" -ArgumentList "clean", $additionalParameter
        $cleanProcess.WaitForExit()
        if (-not $?)
        {
            throw "dotnet clean failed"
        }
    }

    $buildProcess = Start-Process -NoNewWindow -PassThru -FilePath "dotnet" @extras

    $buildProcess.WaitForExit()
    if (-not $?)
    {
        Write-Host $LASTEXITCODE
        throw "dotnet build failed"
    }

}

Build-Dotnet -projPath ./Shared/Lib.Shared.csproj

Build-Dotnet -projPath ./ArgumentParsing/ArgumentParsing.fsproj

Build-Dotnet -projPath ./Frontend/Frontend.csproj

Build-Dotnet -projPath ./Server/Server.csproj

Build-Dotnet -projPath ./Logic.Tests/Logic.Tests.csproj

Set-Location -Path ./SimpleDatabase.Sdk

Build-Dotnet -projPath ./SimpleDatabase.Sdk.sqlproj -additionalParameter /p:NetCoreBuild=true -clean $true

Set-Location -Path ..

dotnet test -c Release --no-build ./Logic.Tests/Logic.Tests.csproj
if (-not $?) {
    throw "Unit Tests failed"
}