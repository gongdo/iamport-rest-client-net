<#
.DESCRIPTION
Invoke-Build trys to build the project with versioning.
#>
Function Invoke-Build {
    Param (
        [string]
        #Specifies the project path to build.
        $ProjectPath,
        [string]
        #Specifies the build number.
        $BuildNumber = "1",
        [string]
        #Specifies the Pull-Request number.
        $PRNumber,
        [string]
        #Specifies the tag/release name.
        $Tag
    )

    Write-Host "ProjectPath: $($ProjectPath)"
    Write-Host "BuildNumber: $($BuildNumber)"
    Write-Host "PRNumber: $($PRNumber)"
    Write-Host "Tag: $($Tag)"

    $ErrorActionPreference = "Stop"
    $Version = "0.0.0"

    if (![System.IO.Directory]::Exists($ProjectPath)) {
        Write-Error "ProjectPath '$($ProjectPath)' not exists"
        exit 1
    }

    if (![string]::IsNullOrEmpty($Tag)) {
        $m = [regex]::Match($Tag, "^(?:v)?([0-9]).([0-9]).([0-9])(?:-((?:[0-9a-zA-Z]+)(?:-[0-9a-zA-Z]+)*))?$")
        if ($m.Success) {
            $c = $m.captures
            $Version = "$($c.groups[1]).$($c.groups[2]).$($c.groups[3])-$($c.groups[4])"
        } else {
            Write-Error "Tag should be a sem-ver with optional leading 'v'"
            exit 1
        }
    } elseif (![string]::IsNullOrEmpty($PRNumber)) {
        $Version = "0.0.0-pr$($PRNumber)-$($BuildNumber)"
    } else {
        $Version = "0.0.0-direct-commit-$($BuildNumber)"
    }

    if ([string]::IsNullOrEmpty($PRNumber)) {
        Write-Host "Build and pack with version '$($Version)'"
        dotnet pack $ProjectPath -c Release /p:Version=$Version
    } else {
        Write-Host "Build with version '$($Version)'"
        dotnet build $ProjectPath -c Release /p:Version=$Version
    }
}
