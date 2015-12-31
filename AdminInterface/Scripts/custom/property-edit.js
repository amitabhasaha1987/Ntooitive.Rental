    function SaveDetails(type, uniqueId) {
        var isFeatured = $("#IsFeatured option:selected").val();
        var isPrinted = $("#IsPrinted option:selected").val();
        var agentDescription = $("#AgentDescription").val();
        var dateRange = $('#open_house_dateRange').val().split('-');
        var isSpotlight = $("#IsSpotlight option:selected").val();

        var formData = {
            UniqueId: uniqueId,
            IsFeatured: isFeatured,
            IsSpotlight: isSpotlight,
            IsPrinted: isPrinted,
            AgentDescription: agentDescription,
            OpenHouseStartDateTimestr: dateRange[0].trim(),
            OpenHouseEndDateTimestr: dateRange[1].trim()
        }; //Array
     
        $.ajax({
            url: "/property/" + type + "/update",
            type: "POST",
            data: formData,
            success: function (data, textStatus, jqXHR) {
                if (data) {
                    $("#sucessalert").show();
                    $('#sucessalert').delay(5000).fadeOut(400);
                } else {
                    $("#failurealert").show();
                    $('#failurealert').delay(5000).fadeOut(400);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#failurealert").show();
                $('#failurealert').delay(5000).fadeOut(400);
            }
        });
    }