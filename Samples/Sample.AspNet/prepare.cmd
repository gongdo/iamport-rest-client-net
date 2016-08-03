@ECHO OFF
SET BuildDirectory=%CD%
SET OutputDirectory=%BuildDirectory%\bin
MD %OutputDirectory%

ECHO "restore packages"
CD "../.."
dotnet restore

ECHO "build sources"
dotnet build src/**/project.json --configuration DEBUG --framework "net45" --output "%OutputDirectory%" --build-base-path "%OutputDirectory%"

CD "%BuildDirectory%"