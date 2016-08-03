## 주의사항
- Iamport.RestApi는 .NET Core 기반의 `xproj` 프로젝트입니다.
- 2016년 8월 현재 `xproj` 프로젝트는 .NET Framework 기반의 `csproj` 프로젝트에서 직접 참조할 수 없습니다.
- 이 문제를 해결하려면 다음의 두 가지 방법이 있습니다.
 - 1. 어셈블리 파일(dll)을 직접 참조
 - 2. NuGet 패키지로 배포하고 Nuget 매니저에서 패키지를 참조
- 여기에서는 샘플 프로젝트이므로 `DEBUG`로 빌드한 어셈블리 파일을 샘플 프로젝트에서 직접 참조하였습니다.
 - 빌드는 pre-build 스텝에서 자동으로 이루어지며 prepare.cmd 스크립트를 실행하여 참조할 어셈블리를 자동으로 빌드합니다.
- 그러나 실제 프로젝트에서는 NuGet에서 배포된 정식 버전 또는 베타 버전을 사용하길 바랍니다.

## 샘플코드
- 모든 샘플은 Sample.AspNetCore를 기준으로 작성합니다.
- 중복되는 코드는 Sample.AspNetCore에서 `Add as Link`로 파일을 참조하며 따라서 링크된 파일의 네임스페이스는 `Sample.AspNetCore`입니다.