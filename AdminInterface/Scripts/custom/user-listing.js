$(function () {
    create_datatable();

    //track search box value 
    $("input[type='search']").keydown(function () {
        $.cookie("search-data", JSON.stringify($("input[type='search']").val()));

    })
    .keypress(function (e) {  //for backspace
        $.cookie("search-data", JSON.stringify($("input[type='search']").val()));
    })
    .on('input paste', function () {
        $.cookie("search-data", JSON.stringify($("input[type='search']").val()));
        console.log($("input[type='search']").val());
    })

    //check cookie and fire search on datatable
    if (typeof $.cookie('search-data') !== 'undefined') {
        var search_value = JSON.parse($.cookie("search-data"));
        var oTable = $('#userlisting').dataTable();
        oTable.fnFilter(search_value);
    }
});
var userstable;
function create_datatable() {
    userstable = $('#userlisting').dataTable({
        "bServerSide": true,
         "scrollX": true,
        "sAjaxSource": "/agent/agent-listing-ajax-handler",
        "bProcessing": true,
        "order": [
            [1, "asc"]
        ],
        "aoColumns": [

            {
                "sName": "ParticipantId",
                "bSearchable": true,
                "bSortable": true
            }, {
                "sName": "FirstName",
                "bSearchable": true,
                "bSortable": true
            }, {
                "sName": "LastName",
                "bSearchable": true,
                "bSortable": true,
            }, {
                "sName": "Email",
                "bSearchable": true,
                "bSortable": true
            }, {
                "sName": "PrimaryContactPhone",
                "bSearchable": true,
                "bSortable": true
            }, {
                "sName": "OfficePhone",
                "bSearchable": true,
                "bSortable": true
            }, {
                "sName": "Roles",
                "bSearchable": false,
                "bSortable": false
            },
         {
             "bSortable": false,
             "bSearchable": false,
             "mRender": function (data, type, full) {
                 return "<a class=\"btn btn-block btn-primary\" href=\"javascript:ManageUser('" + full[0] + "');\">Manage</a>";
             }

         }
         ,
         {
             "bSortable": false,
             "bSearchable": false,
             "mRender": function (data, type, full) {
                 return "<a class=\"btn btn-block btn-danger\" href=\"javascript:DeleteUser('" + full[0] + "');\">Delete</a>";
             }

         }
            //, {
            //    "bSortable": false,
            //    "bSearchable": false,
            //    "mRender": function (data, type, full) {
            //        if (full[8] == "True" && full[7] == "True") {
            //            return "<a  data-toggle=\"tooltip\" data-placement=\"left\" title=\"Click here to deactive the agent login. \" id=\"btndeactive_" + full[0] + "\" class=\"btn btn-block btn-danger\" href=\"javascript:Deactivate('" + full[0] + "',$('#btndeactive_" + full[0] + "'));\">Deactivate</a>";

            //        } else if (full[8] == "True" && full[7] == "False") {
            //            return "<a data-toggle=\"tooltip\" data-placement=\"left\" title=\"Click here to resend activation mail.\" id=\"btnactive_" + full[0] + "\"  class=\"btn btn-block  btn-info\" href=\"javascript:SendActivationMail('" + full[0] + "',$('#btnactive_" + full[0] + "'));\">Pending</a>";

            //        } else if (full[8] == "False" && full[7] == "True") {
            //            return "<a data-toggle=\"tooltip\" data-placement=\"left\" title=\"Click here to deactive the agent login.\" id=\"btndeactive_" + full[0] + "\" class=\"btn btn-block btn-danger\" href=\"javascript:Deactivate('" + full[0] + "',$('#btndeactive_" + full[0] + "'));\">Deactivate</a>";

            //        } else if (full[8] == "False" && full[7] == "False") {
            //            return "<a data-toggle=\"tooltip\" data-placement=\"left\" title=\"Click here to send activation mail.\" id=\"btnactive_" + full[0] + "\" class=\"btn btn-block btn-primary\" href=\"javascript:SendActivationMail('" + full[0] + "',$('#btnactive_" + full[0] + "'));\">Activate</a>";

            //        }

            //    }
            //}
        ]
    });

}

function ManageUser(uniqueid) {
    window.location.href = "/agent/manage/" + uniqueid+"/Agent";
}

function DeleteUser(uniqueid) { 
    $.ajax({
        url: "/agent/delete-agent?agentId=" + uniqueid,
        type: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data) {
                $("#sucessalertdelete").show();
                $('#sucessalertdelete').delay(5000).fadeOut(400);

                var userlisting = $("#userlisting").dataTable({ bRetrieve: true });
                userstable.fnDraw(true);
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
