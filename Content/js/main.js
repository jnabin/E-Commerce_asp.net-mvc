

$(document).ready(function () {
    $('.profile').on('click', function () {
        $(".HoverAbleDivCart").hide();
        $(".HoverAbleDivProfile").toggle();
    });

    $('.cart').on('click', function () {
        $(".HoverAbleDivProfile").hide();
        $(".HoverAbleDivCart").toggle();
    });

    $('#dismiss, .overlay').on('click', function () {
        $('#sidebar').removeClass('active');
        $('.overlay').removeClass('active');
    });
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').addClass('active');
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expand=true').attr('aria-expanded', 'false');
    });
    $('.x0').mouseenter(function () {
        $('.HoverAbleDiv0').stop().show();
    });
    $(".x0, .HoverAbleDiv0").mouseleave(function () {
        if (!$('.HoverAbleDiv0').is(':hover')) {
            $('.HoverAbleDiv0').hide();
        };
    });
    $('.x1').mouseenter(function () {
        $('.HoverAbleDiv1').stop().show();
    });
    $(".x1, .HoverAbleDiv1").mouseleave(function () {
        if (!$('.HoverAbleDiv1').is(':hover')) {
            $('.HoverAbleDiv1').hide();
        };
    });
    $('.x2').mouseenter(function () {
        $('.HoverAbleDiv2').stop().show();
    });
    $(".x2, .HoverAbleDiv2").mouseleave(function () {
        if (!$('.HoverAbleDiv2').is(':hover')) {
            $('.HoverAbleDiv2').hide();
        };
    });
    $('.x3').mouseenter(function () {
        $('.HoverAbleDiv3').stop().show();
    });
    $(".x3, .HoverAbleDiv3").mouseleave(function () {
        if (!$('.HoverAbleDiv3').is(':hover')) {
            $('.HoverAbleDiv3').hide();
        };
    });
    $('.x4').mouseenter(function () {
        $('.HoverAbleDiv4').stop().show();
    });
    $(".x4, .HoverAbleDiv4").mouseleave(function () {
        if (!$('.HoverAbleDiv4').is(':hover')) {
            $('.HoverAbleDiv4').hide();
        };
    });
    $('.x5').mouseenter(function () {
        $('.HoverAbleDiv5').stop().show();
    });
    $(".x5, .HoverAbleDiv5").mouseleave(function () {
        if (!$('.HoverAbleDiv5').is(':hover')) {
            $('.HoverAbleDiv5').hide();
        };
    });
    $('.x6').mouseenter(function () {
        $('.HoverAbleDiv6').stop().show();
    });
    $(".x6, .HoverAbleDiv6").mouseleave(function () {
        if (!$('.HoverAbleDiv6').is(':hover')) {
            $('.HoverAbleDiv6').hide();
        };
    });
});