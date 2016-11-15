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