(function () {
    document.frm_payment.merchant_uid.value = 'merchant_' + new Date().getTime();
    document.frm_payment.vbank_due.value = moment().add(2, 'day').format('YYYYMMDD');

    $('#requester').click(function () {
        var frm = document.frm_payment;
        var IMP = window.IMP;
        var escrow = $(frm.use_escrow).is(':checked');
        var in_app = $(frm.in_app).is(':checked');

        // 지원 pg사 추가 될 때마다 추가 바람.
        switch (frm.pg_provider.value) {
            case 'uplus':
                IMP.init('imp46648599');
                break;
            case 'nice':
                IMP.init('imp84043725');
                break;
            case 'html5_inicis':
                IMP.init('imp68124833');
                break;
            case 'inicis':
                IMP.init('imp20685811');
                break;
            case 'jtnet':
                IMP.init('imp57843720');
                break;
            case 'kakao':
                IMP.init('imp10391932');
                break;
            case 'danal':
                IMP.init('imp00357859');
                break;
            case 'paypal':
                IMP.init('imp09350031');
                break;
            default:
                IMP.init('iamport');
                break;
        }

        var param = {
            pay_method: frm.pay_method.value,
            escrow: escrow,
            merchant_uid: frm.merchant_uid.value,
            name: frm.name.value,
            amount: frm.amount.value,
            buyer_email: frm.buyer_email.value,
            buyer_name: frm.buyer_name.value,
            buyer_tel: frm.buyer_tel.value,
            buyer_addr: frm.buyer_addr.value,
            buyer_postcode: frm.buyer_postcode.value,
            vbank_due: frm.vbank_due.value
        };

        if (in_app) param.app_scheme = 'iamporttest';

        var msg = '';
        IMP.request_pay(param, function (rsp) {
            if (rsp.success) {
                msg = '결제가 완료되었습니다.<br>';
                msg += '고유ID : ' + rsp.imp_uid + '<br>';
                msg += '상점 거래ID : ' + rsp.merchant_uid + '<br>';
                msg += '결제 금액 : ' + rsp.paid_amount + '<br>';

                if (rsp.pay_method === 'card') {
                    msg += '카드 승인번호 : ' + rsp.apply_num + '<br>';
                } else if (rsp.pay_method === 'vbank') {
                    msg += '가상계좌 번호 : ' + rsp.vbank_num + '<br>';
                    msg += '가상계좌 은행 : ' + rsp.vbank_name + '<br>';
                    msg += '가상계좌 예금주 : ' + rsp.vbank_holder + '<br>';
                    msg += '가상계좌 입금기한 : ' + rsp.vbank_date + '<br>';
                }
                $('#responser').addClass('alert-success');
            } else {
                msg = '결제에 실패하였습니다.' + '<br>';
                msg += '에러내용 : ' + rsp.error_msg + '<br>';
                $('#responser').addClass('alert-danger');
            }
            $('#responser').html(msg).show();
            setTimeout(function () {
                $('#responser').hide();
                location.reload();
            }, 100000);
        });
        return false;
    });

    $('#pg_provider').change(function () {
        // 지원 pg사 추가 될 때마다 추가 바람.
        var provider = $(this).val();
        var help_text = '';
        switch (provider) {
            case 'inicis':
            case 'html5_inicis':
                help_text = '실제 승인이 이루어진 테스트 결제건은 자정에 이니시스에서 자동 취소처리합니다.';
                break;

            case 'uplus':
                help_text = '실제 승인이 이루어지지 않기 때문에 청구되지 않습니다.';
                break;

            case 'nice':
                help_text = '실제 승인이 이루어진 테스트 결제건은 자정에 나이스정보통신에서 자동 취소처리합니다.';
                break;

            case 'jtnet':
                help_text = '실제 승인이 이루어진 테스트 결제건은 자정에 JTNet에서 자동 취소처리합니다.';
                break;

            case 'danal':
                help_text = '실제 승인이 이루어진 테스트 결제건은 약 30분 후 자동 취소처리됩니다.';
                break;

            case 'kakao':
                help_text = '실제 승인이 이루어진 테스트 결제건은 30분이내로 카카오페이에서 자동 취소처리 합니다.';
                break;

            default:
                break;
        }
        changeSelectOption();
        $('#pay_method_help').text(help_text);
    });

    var changeSelectOption = function () {
        var payMethod = $('option:selected').data('option').split(',');

        $('#pay_method').empty();

        for (var i in payMethod) {
            switch (payMethod[i]) {
                case 'card':
                    $('#pay_method').append('<option value="card">신용카드</option>');
                    break;
                case 'trans':
                    $('#pay_method').append('<option value="trans">실시간계좌이체</option>');
                    break;
                case 'vbank':
                    $('#pay_method').append('<option value="vbank">가상계좌</option>');
                    break;
                case 'phone':
                    $('#pay_method').append('<option value="phone">휴대폰소액결제</option>');
                    break;
                case 'cultureland':
                    $('#pay_method').append('<option value="cultureland">문화상품권</option>');
                    break;
                case 'smartculture':
                    $('#pay_method').append('<option value="smartculture">스마트문상</option>');
                    break;
                case 'happymoney':
                    $('#pay_method').append('<option value="happymoney">해피머니</option>');
                    break;
                default:
                    break;
            }
        }
    };
}).call(this);