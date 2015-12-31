$(function () {
    //This .change jquery function is the selectedchange event of the Master Level dropdownlist.
    $('#Address_City').change(function () {
        debugger;
        var id = $('#Address_City option:selected').val();// Variable id is storing the selected value of the Master Level dropdownlist
        if (id == "Other") {
            $('#Other_City').css("display", "block");
        } else {
            $('#Other_City').css("display", "none");
        }
        jQuery.ajax(
        {
            type: "get",
            url: "/office/ZipCodeList?CityName=" + id,// Going to the BranchList Action in Branch controller with selected id of the Master Level dropdown
            dataType: "json",
            success: function (data) {//data is json result set that is returning from the BranchList Action method
                $('#Address_PostalCode').empty();// first remove the current options if any in the Parent Level dropdown
                var optionhtml1 = '<option value="' +
                 0 + '">' + "--All ZipCode--" + '</option>';
                $("#Address_PostalCode").append(optionhtml1);// Binding the first option for Parent Level dropdown
                $.each(data, function (index, optiondata) {// next iterate through JSON (result set) object adding each option to the Parent Level dropdown
                    $("#Address_PostalCode").append("<option value='" + optiondata.PostalCode + "'>" + optiondata.PostalCode + "</option>");
                });
            }
        });
    });
});