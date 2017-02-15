# iamport-rest-client-net
[아임포트](http://www.iamport.kr/)는 한국 Payment Gateway(PG)사의 복잡한 구현을 간소화주는 서비스로 PG사의 UI나 플러그인을 거치지 않고 처리할 수 있는 몇 가지 기능을 REST API로 제공합니다.

이 프로젝트는 [아임포트(I'mport) REST API](https://api.iamport.kr/) 클라이언트의 닷넷 구현입니다. 프로젝트 이름을 표기할 때에는 `아임포트RESTAPI닷넷`이라고 합니다.

아임포트RESTAPI닷넷은 모던한 닷넷 플랫폼을 지향하며 닷넷 4.5 이상을 대상으로 합니다. 네트워크를 통한 모든 호출은 `async` 비동기로 이루어집니다.


## Quick start

### Installation

> **Note**

> 2017-02-15 현재, `subscribe-api` 기능을 테스트중입니다.

> `subscribe-api`를 사용하지 않을 경우 `1.0.0` 버전을 사용하면 됩니다.

(패키지 매니저 콘솔에서)
```
Install-Package Iamport.RestApi -Version 1.1.0-32
```

(또는 project.json에 추가 - .NET Core xproj 프로젝트)
```
{
    "dependencies": {
        "Iamport.RestApi": "1.1.0-32"
    }
}
```

### Usage - 결제 플러그인 방식

```CSharp
using Iamport.RestApi;
using Iamport.RestApi.Apis;
using Iamport.RestApi.Models;
//...

var options = new IamportHttpClientOptions
{
    ApiKey = "{your API key}",
    ApiSecret = "{your API secret}",
    IamportId = "{your Iamport Id}"
};
var httpClient = new IamportHttpClient(options);
var paymentsApi = new PaymentsApi(httpClient);

// 지정한 거래 ID와 금액을 아임포트에 등록합니다.
// 등록된 거래 ID는 반드시 동일한 금액만 결제되도록 보장합니다.
await paymentsApi.PrepareAsync(new PaymentPreparation
{
    Amount = 1000,
    TransactionId = "1234567890"
});

// 이후 HTML페이지에서 결제 진행...

// 완료된 결제 내용 조회(by 거래 ID)
var result = await paymentsApi.GetByTransactionIdAsync("1234567890");
// 또는 아임포트 고유 ID로 조회
var result2 = await paymentsApi.GetByIamportIdAsync("IMP12345");
```

### Usage - 비인증 API 결제

`iamport`의 [subscribe-api](http://api.iamport.kr/#!/subscribe/onetime)를 사용한 결제입니다.

```
// iamportClient 생성은 위와 동일
var subscribeApi = new SubscribeApi(iamportClient);

var request = new DirectPaymentRequest
{
    CustomerId = null,  // (customer_uid) 등록된 사용자 빌링키 없이 한번 진행
    TransactionId = "merchant_uid",
    Amount = 1004,
    Title = "상품명",
    CardNumber = "dddd-dddd-dddd-dddd",
    Expiry = "yyyy-mm,
    AuthenticationNumber = "dddddd",    // (birth) 생년월일 앞 6자리 또는 법인번호
    PartialPassword = "dd", // (pwd_2digit) 카드비밀번호 앞 2자리, 법인카드일 경우 생략
};

Payment iamportPayment = null;
try
{
    iamportPayment = await subscribeApi.DoDirectPaymentAsync(request);
    // 결과는 payments-api에서 얻은 payment 정보와 동일
}
catch (IamportResponseException ex)
{
    // 결제에 실패하였거나 통신에 오류가 있었을 경우
    Trace.WriteLine("결제 실패 사유: " + ex.Message);
}
```


## Status

|Type|Status|
|---|---|
| CI Build | [![Build status](https://ci.appveyor.com/api/projects/status/icygwugodo4jalcs?svg=true)](https://ci.appveyor.com/project/gongdo/iamport-rest-client-net)
| MyGet Pre-release | [![MyGet](https://img.shields.io/myget/bapul/v/Iamport.RestApi.svg)](https://www.myget.org/feed/bapul/package/nuget/Iamport.RestApi)
| NuGet Release | [![NuGet](https://img.shields.io/nuget/v/Iamport.RestApi.svg)](https://www.nuget.org/packages/Iamport.RestApi/)

### Pre-release
Pre-release 버전은 [MyGet](https://www.myget.org)으로 배포합니다.

- NuGet V3 Feed: https://www.myget.org/F/bapul/api/v3/index.json
- NuGet V2 Feed: https://www.myget.org/F/bapul/api/v2

## Roadmap
아임포트RESTAPI닷넷은 다음과 같이 진행됩니다. 각 마일스톤의 일정은 아직 미정입니다.
아임포트RESTAPI닷넷의 버전 규칙은 [Semantic Versioning](http://semver.org/)을 따릅니다.

* [1.1](https://github.com/gongdo/iamport-rest-client-net/milestones/1.1.0)
 * Subscription API 추가
* 1.2
 * SMS 본인 인증 추가

## Contribution
이 프로젝트는 누구나 참여할 수 있는 오픈소스 프로젝트입니다. 참여와 기여는 다음과 같이 할 수 있습니다.

### 각 결제 서비스별 테스트
결제서비스는 실제로 동작을 해보기 전까지는 제대로 동작하는지 확인하기 어렵습니다. 현재 동작이 확인된 서비스는 다음과 같습니다. 다른 결제사에서의 테스트 결과가 있으면 알려주시기 바랍니다.

- 이니시스(구버전)
- 이니시스(웹표준버전)

### 이슈 제기
이 프로젝트에 관한 문제점, 문의, 개선 사항 건의 등은 [Issues](https://github.com/gongdo/iamport-rest-client-net/issues)에 등록해주세요. 단, 이슈를 등록할 때에는 충분히 검색하여 같은 이슈가 없었는지 확인을 바랍니다.

### Development Environment Requirement
현재 아임포트RESTAPI닷넷은 다음과 같은 환경에서 개발됩니다.
- [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx) - IDE
- [.NET Core 1.0](https://www.microsoft.com/net/core#windows) - Runtime
- [xunit](https://xunit.github.io) - 단위 테스트 및 기능 테스트.
 
.NET Core 프로젝트이므로 MAC OSX와 Linux에서도 개발 및 테스트가 가능합니다.

### Pull Request(PR)
PR은 온전한 단위 테스트 및 기능 테스트셋을 포함해야 하며 해결하려는 목적이 명확해야 합니다

## License
이 프로젝트는 [아파치 라이선스 2.0](https://github.com/gongdo/iamport-rest-client-net/blob/master/LICENSE)을 따르며 이에 따라 모든 코드는 자유롭게 수정, 배포, 이용이 가능합니다.

[아임포트(I'mport)](http://www.iamport.kr/) 및 관련 상표는 아임포트에 권리가 있으며 이 프로젝트는 소스코드의 이용으로 인해 발생할 수 있는 문제에 대해 일체의 책임을 지지 않습니다.
