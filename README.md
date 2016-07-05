

# iamport-rest-client-net
[아임포트](http://www.iamport.kr/)는 한국 Payment Gateway(PG)사의 복잡한 구현을 간소화주는 서비스로 PG사의 UI를 거치지 않고 처리할 수 있는 몇 가지 기능을 REST API로 제공합니다.

이 프로젝트는 [아임포트(I'mport) REST API](https://api.iamport.kr/) 클라이언트의 닷넷 구현입니다. 프로젝트 이름을 표기할 때에는 `아임포트RESTAPI닷넷`이라고 합니다.

아임포트RESTAPI닷넷은 모던한 닷넷 플랫폼을 지향하며 최소 닷넷 4.5 이상을 대상으로 합니다.


## Quick start
*아직 작성되지 않았습니다.*

## Status
[![Build status](https://ci.appveyor.com/api/projects/status/icygwugodo4jalcs?svg=true)](https://ci.appveyor.com/project/gongdo/iamport-rest-client-net)
[![MyGet](https://img.shields.io/myget/bapul/v/Iamport.RestApi.svg)](https://www.myget.org/feed/bapul/package/nuget/Iamport.RestApi)
[![NuGet](https://img.shields.io/nuget/v/Iamport.RestApi.svg)](https://www.nuget.org/packages/Iamport.RestApi/)

## Roadmap
아임포트RESTAPI닷넷은 다음과 같이 진행됩니다. 각 마일스톤의 일정은 아직 미정입니다.
아임포트RESTAPI닷넷의 버전 규칙은 [Semantic Versioning](http://semver.org/)을 따릅니다. 마일스톤에는 revision이나 build버전을 붙이지 않습니다.

* [1.0-preview1]
 * 샘플 프로젝트 제공
 * .NET 4.5 지원
 * .NET Core 1.0 지원
* [1.0](https://github.com/gongdo/iamport-rest-client-net/milestones/1.0)
 * 마이너 버그 수정
 * 최초 릴리스!

## Contribution
이 프로젝트는 누구나 참여할 수 있는 오픈소스 프로젝트입니다. 참여와 기여는 다음과 같이 할 수 있습니다.

### 이슈 제기
이 프로젝트에 관한 문제점, 문의, 개선 사항 건의 등은 [Issues](https://github.com/gongdo/iamport-rest-client-net/issues)에 등록해주세요. 단, 이슈를 등록할 때에는 충분히 검색하여 같은 이슈가 없었는지 확인을 바랍니다.

### Development Environment Requirement
현재 아임포트RESTAPI닷넷은 다음과 같은 환경에서 개발됩니다.
* .NET Core 기반 프로젝트
* [xunit](xunit.github.io) - 단위 테스트 및 기능 테스트.
 
.NET Core 프로젝트이므로 MAC OSX와 Linux에서도 개발 및 테스트가 가능합니다.

### Pull Request(PR)
PR은 온전한 단위 테스트 및 기능 테스트셋을 포함해야 하며 해결하려는 목적이 명확해야 합니다. 모든 PR이 받아들여지는 것은 아니며 PR의 리뷰에 걸리는 시간을 보장하지 않습니다.

### Code Convention
*아직 작성되지 않았습니다.*

## License
이 프로젝트는 [아파치 라이선스 2.0](https://github.com/gongdo/iamport-rest-client-net/blob/master/LICENSE)을 따르며 이에 따라 모든 코드는 자유롭게 수정, 배포, 이용이 가능합니다.

[아임포트(I'mport)](http://www.iamport.kr/) 및 관련 상표는 아임포트에 권리가 있으며 이 프로젝트는 소스코드의 이용으로 인해 발생할 수 있는 문제에 대해 일체의 책임을 지지 않습니다.
