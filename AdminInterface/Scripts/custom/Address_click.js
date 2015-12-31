$(function () {
    //This .change jquery function is the selectedchange event of the Master Level dropdownlist.
    $('#Address_FullStreetAddress').change(function () {
        debugger;
        var id = $('#Address_FullStreetAddress option:selected').val();// Variable id is storing the selected value of the Master Level dropdownlist
        if (id == "Other") {
            $('#Other_FullStreetAddress').css("display", "block");
        } else {
            $('#Other_FullStreetAddress').css("display", "none");
        }
    });
});