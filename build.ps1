 param (
    [string]$task = "build"
 )

if($task -eq "build"){
    
    Push-Location src/Operation
    dotnet restore
    dotnet build
    Pop-Location
}

if($task -eq "test"){
    Push-Location test/Tests
    dotnet restore
    dotnet test
    Pop-Location
}

if($task -eq "pack"){
    Push-Location src/Operation
    dotnet pack --configuration Release
    Pop-Location
}