$(function () {
    //This .change jquery function is the selectedchange event of the Master Level dropdownlist.
    $('#Role').change(function () {
        debugger;
        var id = $('#Role option:selected').val();// Variable id is storing the selected value of the Master Level dropdownlist
        if (id == "Builder" || id=="Admin") {
            $('#offcDiv').hide();
        } else {
            $('#offcDiv').show();
        }
    });
});