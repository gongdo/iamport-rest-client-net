이슈 트래킹: [![Stories in Ready](https://badge.waffle.io/gongdo/iamport-rest-client-net.png?label=ready&title=Ready)](https://waffle.io/gongdo/iamport-rest-client-net)
# iamport-rest-client-net
[아임포트](http://www.iamport.kr/)는 한국 Payment Gateway(PG)사의 복잡한 구현을 간소화주는 서비스로 PG사의 UI를 거치지 않고 처리할 수 있는 몇 가지 기능을 REST API로 제공합니다.

이 프로젝트는 [아임포트(I'mport) REST API](https://api.iamport.kr/) 클라이언트의 닷넷 구현입니다. 프로젝트 이름을 표기할 때에는 `아임포트RESTAPI닷넷`이라고 합니다.

아임포트RESTAPI닷넷은 모던한 닷넷 플랫폼을 지향하며 최초 개발은 [DNX(a .NET Execution Environment)](https://github.com/aspnet/dnx) 4.5.1 및 DNX Core 5.0을 목표로 합니다. DNX에서의 구현이 안정화되면 다음으로 .NET 4.6을 목표로 합니다. 이는 모던 닷넷 플랫폼을 우선적으로 지원하는 것이 목표이기 때문이며 레거시 플랫폼(.NET 1.1, 2.0, 3.0 등)도 기여자가 있다면 진행할 수 있을 것입니다.


## Quick start
*아직 작성되지 않았습니다.*

## Roadmap
아임포트RESTAPI닷넷은 다음과 같이 진행됩니다. 각 마일스톤의 일정은 아직 미정입니다.

* [1.0-beta1](https://github.com/gongdo/iamport-rest-client-net/milestones/1.0-beta1) (**현재 진행중**)
 * DNX 4.5.1과 DNX Core 5.0에서 구동하는 코드 작성
 * Users API, Payments API 제공
 * 샘플 프로젝트 제공
 * 단위 테스트, 기능 테스트 및 CI build 구성
 * Nuget 패키지 제공
* [1.0-rc1](https://github.com/gongdo/iamport-rest-client-net/milestones/1.0-rc1)
 * .NET 4.6 지원
 * .NET 4.0, 4.5 지원(*미정*)
 * Subscribe API 지원
 * 메이저 버그 수정
* [1.0](https://github.com/gongdo/iamport-rest-client-net/milestones/1.0)
 * 마이너 버그 수정
 * 최초 릴리스!

## Contribution
이 프로젝트는 누구나 참여할 수 있는 오픈소스 프로젝트입니다. 참여와 기여는 다음과 같이 할 수 있습니다.

### 이슈 제기
이 프로젝트에 관한 문제점, 문의, 개선 사항 건의 등은 [Issues](https://github.com/gongdo/iamport-rest-client-net/issues)에 등록해주세요. 단, 이슈를 등록할 때에는 충분히 검색하여 같은 이슈가 없었는지 확인을 바랍니다.

### Development Environment Requirement
현재 아임포트RESTAPI닷넷은 다음과 같은 환경에서 개발됩니다.
* ASP.NET 5 (beta8) [OS별 개발환경 소개](https://docs.asp.net/en/latest/getting-started/index.html) 참고. 
* DNX 4.5.1과 DNX core 5.0
* [xunit](xunit.github.io) - 단위 테스트 및 기능 테스트.
 
DNX 프로젝트이므로 MAC OSX와 Linux에서도 [Mono](http://www.mono-project.com/)를 설치후 개발 및 테스트가 가능합니다.

### Pull Request(PR)
PR은 온전한 단위 테스트 및 기능 테스트셋을 포함해야 하며 해결하려는 목적이 명확해야 합니다. 모든 PR이 받아들여지는 것은 아니며 PR의 리뷰에 걸리는 시간을 보장하지 않습니다.

### Code Convention
*아직 작성되지 않았습니다.*

## License
이 프로젝트는 [아파치 라이선스 2.0](https://github.com/gongdo/iamport-rest-client-net/blob/master/LICENSE)을 따르며 이에 따라 모든 코드는 자유롭게 수정, 배포, 이용이 가능합니다.

[아임포트(I'mport)](http://www.iamport.kr/) 및 관련 상표는 아임포트에 권리가 있으며 이 프로젝트는 소스코드의 이용으로 인해 발생할 수 있는 문제에 대해 일체의 책임을 지지 않습니다.
