$(function () {
    //This .change jquery function is the selectedchange event of the Master Level dropdownlist.
    $('#Address_StateOrProvince').change(function () {
        debugger;
        var id = $('#Address_StateOrProvince option:selected').val();// Variable id is storing the selected value of the Master Level dropdownlist
        if (id == "Other") {
            $('#Other_StateOrProvince').css("display", "block");
        } else {
            $('#Other_StateOrProvince').css("display", "none");
        }
        jQuery.ajax(
        {
            type: "GET",
            url: "/office/citylist?StateName=" + id,// Going to the BranchList Action in Branch controller with selected id of the Master Level dropdown
            dataType: "json",
            success: function (data) {//data is json result set that is returning from the BranchList Action method
                $('#Address_City').empty();// first remove the current options if any in the Parent Level dropdown
                var optionhtml1 = '<option value="' +
                 0 + '">' + "--All City--" + '</option>';
                $("#Address_City").append(optionhtml1);// Binding the first option for Parent Level dropdown
                $.each(data, function (index, optiondata) {// next iterate through JSON (result set) object adding each option to the Parent Level dropdown
                    $("#Address_City").append("<option value='" + optiondata.City + "'>" + optiondata.City + "</option>");
                });
            }
        });
    });
});