layui.config({
    base: '/themes/js/modules/'
});
layui.use(['jquery', 'form', 'common'], function () {
    var form = layui.form,
        $ = layui.jquery,
        os = layui.common;
    $(".layui-btn-danger").click(function () {
        document.getElementById("forms").reset();
    });
    //清空token
    os.SessionRemove('FYTADMIN_ACCESS_TOKEN');
    var captcha = $('#botdetect-captcha').captcha({ captchaEndpoint: '/captcha.ashx' });
    form.on('submit(loginsub)', function (data) {
        var crypt = new JSEncrypt();
        crypt.setPrivateKey(data.field.privateKey);
        var enc = crypt.encrypt(data.field.password);
        $("#password").val(enc);
        data.field.password = enc;
        data.field.code = $('#code').val();
        data.field.cid = captcha.getCaptchaId();
        var btns = $(".layui-btn-normal");
        btns.html('<i class="layui-icon layui-anim layui-anim-rotate layui-anim-loop"></i>');
        btns.attr('disabled', 'disabled');
        os.ajax('api/admin/login', data.field, function (res) {
            if (res.statusCode === 200) {
                os.SetSession('FYTADMIN_ACCESS_TOKEN', res.data);
                setTimeout(function () {
                    var rurl = os.getUrlParam('ReturnUrl');
                    if (!rurl) {
                        window.location.href = '/fytadmin/index';
                    }
                    else {
                        window.location.href = rurl;
                    }
                }, 1000);
            } else {
                $(".login-tip span").html(res.message);
                if (res.statusCode===406) {
                    $("#password").val('');
                }
                $(".login-tip").animate({ 'height': '30px' });
                setTimeout(function () {
                    $(".login-tip").animate({ 'height': 0 });
                    $(".login-tip span").html('');
                    if (res.statusCode === 400) {
                        captcha.reloadImage();
                        $('#code').val('');
                    }
                }, 2500);
            }
            btns.attr('disabled', false);
            setTimeout(function () {
                btns.html('登录');
            }, 1000);
        });
        return false;
    });
    $(window).resize(bodysize);
    bodysize();
    function bodysize() {
        $("body").height($(window).height());
    }
    document.getElementById('loginname').focus();
    $('#botdetect-captcha-container').on('click', '#LoginCaptchaStyle_CaptchaImage_HelpLink, #cap_tips', function () {
        captcha.reloadImage();
        return false;
    }).on('mouseenter', '#LoginCaptchaStyle_CaptchaImage_HelpLink', function () {
        $(this).attr('title', '点击刷新验证码');
    });
});