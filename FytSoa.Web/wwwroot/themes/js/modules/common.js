﻿layui.define(['layer', 'toastr'], function (exports) {
    "use strict";

    var $ = layui.jquery,
        layer = layui.layer,
        toastr = layui.toastr;
    toastr.options = {
        "positionClass": "toast-top-right",
        "timeOut": "1500"
    };

    function to_num(number, decimals) {
        var n = !isFinite(+number) ? 0 : +number,
            prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
            s = '',
            toFixedFix = function (n, prec) {
                var k = Math.pow(10, prec);
                return '' + Math.ceil(n * k) / k;
            };

        s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
        var re = /(-?\d+)(\d{3})/;
        while (re.test(s[0])) {
            s[0] = s[0].replace(re, "$1,$2");
        }

        if ((s[1] || '').length < prec) {
            s[1] = s[1] || '';
            s[1] += new Array(prec - s[1].length + 1).join('0');
        }
        return s.join('.');
    }

    var tmls, tool = {
        error: function (msg) {
            toastr.error(msg);
        },
        warning: function (msg) {
            toastr.warning(msg);
        },
        success: function (msg) {
            toastr.success(msg);
        },
        ajax: function (url, options, callFun, method = 'post') {
            var httpUrl = "/", token = tool.GetSession('FYTADMIN_ACCESS_TOKEN');
            var _headers = {};
            if (token !== null) {
                _headers = {
                    'Authorization': 'Bearer ' + token
                };
            }
            options = method === 'get' ? options : JSON.stringify(options);
            //console.log(_headers);
            //console.log(options);
            $.ajax(httpUrl + url, {
                data: options,
                contentType: 'application/json',
                dataType: 'json', //服务器返回json格式数据
                type: method, //HTTP请求类型
                timeout: 10 * 1000, //超时时间设置为50秒；
                headers: _headers,
                success: function (data) {
                    if (data.statusCode === 408 || data.statusCode === 407) {
                        window.location.href = '/fytadmin/login?ReturnUrl=' + window.location.href;
                    } else {
                        callFun(data);
                    }
                },
                error: function (xhr, type, errorThrown) {
                    if (type === 'timeout') {
                        tool.error('连接超时，请稍后重试！');
                    } else if (type === 'error') {
                        //tool.error('连接异常，请稍后重试！');
                        layer.confirm('连接异常，请重新登录', ['确定'], () => window.location.href = '/fytadmin/login?ReturnUrl=' + window.location.href);
                    }
                }
            });
        },
        Open: function (title, url, width, height, fun) {
            top.layer.open({
                type: 2,
                title: title,
                shadeClose: false,
                shade: 0.2,
                //move:false,
                skin: 'layer-cur-open',
                maxmin: false, //开启最大化最小化按钮
                area: [width, height],
                content: url + (url.indexOf('?') > 0 ? '&__f=open' : '?__f=open'),
                zIndex: "10000",
                end: function () {
                    if (fun) fun();
                }
            });
        },
        OpenRight: function (title, url, width, height, fun, cancelFun) {
            var index = layer.open({
                title: title
                , type: 2
                , area: [width, height]
                , shade: [0.1, '#333']
                , resize: false
                , move: false
                , anim: -1
                , offset: 'rb'
                , zIndex: "1000"
                , shadeClose: false
                , skin: 'layer-anim-07'
                , content: url
                , end: function () {
                    if (fun) fun();
                }
                , cancel: function (index) {
                    if (cancelFun) cancelFun(index);
                }
            });
            return index;
        },
        getToken: function () {
            var token = tool.GetSession('FYTADMIN_ACCESS_TOKEN');
            return { 'Authorization': 'Bearer ' + token };
        },
        closeOpen: function () {
            layer.closeAll();
        },
        tableLoading: function () {
            tmls = layer.msg('<i class="layui-icon layui-icon-loading layui-icon layui-anim layui-anim-rotate layui-anim-loop"></i> 正在加载数据哦', { time: 20000 });
        },
        tableLoadingClose: function () {
            setTimeout(function () {
                layer.close(tmls);
            }, 500);
        },
        load: function () {
            $('body').append('<div class="loader-cur-wall"><div class="loader-cur"></div></div>');
        },
        loadClose: function () {
            setTimeout(function () {
                $('.loader-cur-wall').remove();
            }, 100);
        },
        getUrlParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r !== null) return unescape(r[2]); return null;
        },
        formatdate: function (str) {
            if (str) {
                var d = eval('new ' + str.substr(1, str.length - 2));
                var ar_date = [
                    d.getFullYear(), d.getMonth() + 1, d.getDate()
                ];
                for (var i = 0; i < ar_date.length; i++) {
                    var _data = ar_date[i];
                    ar_date[i] = _data < 10 ? '0' + _data.toString() : _data;
                }
                return ar_date.slice(0, 3).join('-') + ' ' + ar_date.slice(3).join(':');
            } else {
                return "无信息";
            }
        },
        SetSession: function (key, options) {
            localStorage.setItem(key, JSON.stringify(options));
        },
        GetSession: function (key) {
            var obj = localStorage.getItem(key);
            if (obj !== null) {
                return JSON.parse(obj);
            }
            return null;
        },
        /**
         * 删除键值对json
         * @param {key} key : 键
         */
        SessionRemove: function (key) {
            localStorage.removeItem(key);
        },
        /**
         * 打印日志到控制台
         * @param {data} data : Json
         */
        log: function (data) {
            console.log(JSON.stringify(data));
        },
        cloudFile: function () {
            $(".fyt-cloud").click(function () {
                var input_text = $(this).data("text");
                var showImg = $(this).data('img');
                var type = $(this).data('type'); //edit=编辑器  sign=默认表单  iframe=弹出层  form=带图片显示
                var frameId = window.frameElement && window.frameElement.id || '', frameUrl = '';
                if (frameId) {
                    frameUrl = '&frameid=' + frameId;
                }
                tool.Open('媒体资源库', '/fytadmin/file/cloud/?type=' + type + '&img=' + showImg + '&control=' + input_text + frameUrl, '950px', '600px');
            });
        },
        isExtImage: function (name) {
            var imgExt = new Array(".png", ".jpg", ".jpeg", ".bmp", ".gif");
            name = name.toLowerCase();
            var i = name.lastIndexOf(".");
            var ext;
            if (i > -1) {
                ext = name.substring(i);
            }
            for (var j = 0; j < imgExt.length; j++) {
                if (imgExt[j] === ext)
                    return true;
            }
            return false;
        },
        tc: function (data, field) { // 货币格式化
            var val = isNaN(data) ? data[field] : data;
            return '￥' + to_num(val / 100, 2);
        },
        ntc: function (data, field) { // 货币格式化
            var val = isNaN(data) ? data[field] : data;
            return '￥' + to_num(val, 2);
        },
        tn: function (data, field) { // 数字格式化
            var val = isNaN(data) ? data[field] : data;
            return to_num(val, 0);
        },
        js: function (jqueryForm) {
            var data = {};
            jqueryForm.serializeArray().forEach((i) => {
                data[i.name] = i.value;
            });
            return data;
        },
        ts: function (data, field) {
            if (field === undefined) {
                field = 'status';
            }
            return '<span class="layui-badge-dot layui-bg-' + (data[field] ? 'blue' : 'gray') + '"></span>';
        },
        tp: function (data, field) {
            var val = isNaN(data) ? data[field] : data;
            return (val * 100).toFixed(2) + '%';
        }
    };
    exports('common', tool);
});


