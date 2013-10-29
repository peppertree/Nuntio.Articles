$(document).ready(function () {

    $("a.fancyimage").fancybox({
        'transitionIn': 'elastic',
        'transitionOut': 'elastic',
        'speedIn': 600,
        'speedOut': 200,
        'overlayShow': false,
        'titlePosition': 'over',
        'centerOnScroll': true,
        'hideOnContentClick': true,
        'overlayShow': true,
        'overlayOpacity': '0.9',
        'overlayColor': '#333'
    });

    var text = $('.break').html();
    var pages = text.split('<!-- PAGE BREAK -->');
    if (pages.length <= 1 || $.query.get('page') == 'all') {
        $('.break').show();
    } else {
        var thisPage = parseInt($.query.get('page')) - 1;
        if (isNaN(thisPage)) thisPage = 0;
        if (thisPage < 0) thisPage = 0;
        if (thisPage >= pages.length) thisPage = 0;
        $('.break').html(pages[thisPage]).show();
        $('.break > h1').after('<h4 class="page-number"></h4>').addClass('has-byline');
        $('h4.page-number').html('Page ' + (thisPage + 1) + ' of ' + pages.length);
        $('.break').append('<div id="nuntio-pagination-nav"></div>');

        if (thisPage - 1 >= 0) {
            var prevPage = thisPage;
            $('#nuntio-pagination-nav').append('<li><a href="?page=' + prevPage + '"><span>&laquo;</span></a></li>');
        }
        $.each(pages, function (i) {
            curPage = i + 1;
            if (i == thisPage) {
                $('#nuntio-pagination-nav').append('<li><a href="?page=' + curPage + '" class="active"><span>' + curPage + '</span></a></li>');
            } else {
                $('#nuntio-pagination-nav').append('<li><a href="?page=' + curPage + '"><span>' + curPage + '</span></a></li>');
            }
        });
        if (thisPage + 2 <= pages.length) {
            var nextPage = thisPage + 2;
            $('#nuntio-pagination-nav').append('<li><a href="?page=' + nextPage + '"><span>&raquo;</span></a></li>');
        }
        $('#nuntio-pagination-nav').append('<li><a href="?page=all" class="full"><span>Full article</span></a></li>');
    }
});