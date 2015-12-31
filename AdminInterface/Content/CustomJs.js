function removeNestedForm(element, container, deleteElement) {
    $container = $(element).parents(container);
    $container.find(deleteElement).val('True');
    $container.hide();
}

//...previous code

function addNestedForm(container, counter, ticks, content) {
    var nextIndex = $(counter).length;
    var pattern = new RegExp(ticks, "gi");
    content = content.replace(pattern, nextIndex);
    $(container).append(content);
}


(function ($) {
   
    $('#baths_up').on('click', function () {
        var $this = $(this);
        console.log($this);
        $('#PlanModelViewList_0__Baths').val(parseInt($('#PlanModelViewList_0__Baths').val(), 10) + 1);
    });
    $('#baths_dw').on('click', function () {
        $('#PlanModelViewList_0__Baths').val(parseInt($('#PlanModelViewList_0__Baths').val(), 10) - 1);
    });
})(jQuery);