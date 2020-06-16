var main_vm = new Vue({
    el: '#mainapp',
    data: {
        menulist: [],
        messList: [],
        messCount: 0,
        isDown: false,
        siteList: [],
        block: false,
        unreadquantity: 0
    },
    created: function () {
        var that = this;
        document.querySelector('body').addEventListener('click', function (e) {
            if (e.target.id === 'notificdown' || e.target.id === 'notificicon') {
                that.isDown = true;
            } else {
                that.isDown = false;
            }
        });
    },
    methods: {
        clearCache: function () {
            os.success('这里是测试，可以ajax到后台清除网站缓存');
        },
        godown: function () {
            this.isDown = this.isDown ? false : true;
        },
        logout: function () {
            os.load();
            os.ajax('api/admin/logout', null, function (res) {
                if (res.statusCode === 200) {
                    window.location.href = res.data;
                } else {
                    os.error(res.message);
                }
            });
        },
        updatepwd: function () {
            os.Open('修改密码', '/fytadmin/updatepwd', '500px', '340px');
        },
        updateloginsum: function () {
            os.ajax('api/admin/updateloginsum', null, function () { });
        },
        unreadcount: function () {
            os.ajax('api/notice/unreadquantity', {}, function (res) {
                if (res.statusCode === 200) {
                    if (parseInt(res.data) > 0) {
                        main_vm.block = true;
                    } else {
                        main_vm.block = false;
                    }
                    main_vm.unreadquantity = res.data;
                } else {
                    os.error(res.message);
                }
            });
        },
        noticelist: function () {
            os.Open('消息列表', '/fytadmin/noticelist', '600px', '400px', main_vm.unreadcount);
        },
        qhSite: function (m) {
            os.load();
            os.ajax('api/admin/rep/site', m, function (res) {
                if (res.statusCode === 200) {
                    window.location.reload();
                } else {
                    os.error(res.message);
                }
            });
        }
    }
});

var os, rm_vm = new Vue({
    el: '#rmapp',
    data: {
        tmlist: [],
        list: []
    }
});
layui.config({
    base: '/themes/js/modules/'
}).use(['element', 'layer', 'jquery', 'common'], function () {
    var element = layui.element, $ = layui.jquery;
    os = layui.common;
    //加载站点列表
    os.ajax('api/cmssite/list', {}, function (res) {
        if (res.statusCode === 200) {
            main_vm.siteList = res.data;
        } else {
            os.error(res.message);
        }
    });
    os.ajax('api/menu/authmenu', null, function (res) {
        if (res.statusCode === 200) {
            $.each(res.data, function (index, item) {
                if (item.layer === 1) {
                    main_vm.menulist.push(item);
                }
            });
            rm_vm.tmlist = main_vm.menulist;
            rm_vm.list = res.data;
            main_vm.$nextTick(function () {
                element.render();
                //定位到菜单
                $(".layui-bg-black .fyt-nav-tree li a").each(function () {
                    if (window.location.pathname === $(this).attr('href')) {
                        $(".layui-bg-black .fyt-nav-tree li").removeClass('layui-nav-itemed');
                        $(this).parents('.layui-nav-item').addClass('layui-nav-itemed');
                        $(this).parent().addClass('layui-this');
                    }
                });
                $(".load8").fadeOut(200);
                element.on('nav(topmenu)', function (elem) {
                    var that = $(elem);
                    var topIndex = that.data('index');
                    $("#rmapp .layui-side-scroll ul").addClass('layui-hide');
                    $("#rmapp .layui-side-scroll ul").eq(topIndex).removeClass('layui-hide');
                });
                $("#rmapp .layui-side-scroll ul").eq(0).removeClass('layui-hide');
            });
        } else {
            $(".load8").fadeOut(200);
            os.error(res.message);
        }
    }, 'get');
    os.ajax('api/admin/loginsum', null, function (res) {
        if (res) {
            os.Open('修改密码', '/fytadmin/updatepwd', '500px', '340px', main_vm.updateloginsum);
        }
    });
    $("#content-container").height($("#main-container").height() - 36);
    main_vm.unreadcount();
});