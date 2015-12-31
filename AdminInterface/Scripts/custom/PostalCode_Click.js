$(function () {
    //This .change jquery function is the selectedchange event of the Master Level dropdownlist.
    $('#Address_PostalCode').change(function () {
        debugger;
        var id = $('#Address_PostalCode option:selected').val();// Variable id is storing the selected value of the Master Level dropdownlist
        if (id == "Other") {
            $('#Other_PostalCode').css("display", "block");
        } else {
            $('#Other_PostalCode').css("display", "none");
        }
    });
});