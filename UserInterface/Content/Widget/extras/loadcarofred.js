
$(function () {
    debugger;

    setTimeout(loadCarouselagent, 1000);

    function loadCarouselagent() {
        if ($('#ntooitive-slider-agent').length != 0) {
            $('#ntooitive-slider-agent').carouFredSel({
                auto: false,
                height: 'auto',
                prev: '#ntooitive-prev-agent',
                next: '#ntooitive-next-agent',
                mousewheel: true,
                responsive: true
            })
        }
    }


});