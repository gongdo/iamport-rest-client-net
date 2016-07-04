# taken from http://andrewlock.net/publishing-your-first-nuget-package-with-appveyor-and-myget/

<#  
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec  
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

# keep current directory
$buildDirectory = Get-Location

Write-Host "clean artifacts folder"
if(Test-Path ./artifacts) { Remove-Item ./artifacts -Force -Recurse }

Write-Host "restore packages"
exec { & dotnet restore }

Write-Host "build them all"
exec { & dotnet build ./**/project.json }

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];

Write-Host "test"
exec { & cd ./test/Iamport.RestApi.Tests }
exec { & dotnet test -c Release }
Set-Location $buildDirectory

Write-Host "pack nuget"
exec { & dotnet pack ./src/Iamport.RestApi -c Release -o ./artifacts --version-suffix=$revision }