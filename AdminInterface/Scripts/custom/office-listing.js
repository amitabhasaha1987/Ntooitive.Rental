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
    $('#userlisting').on('click', 'tbody td img', function () {
       debugger;
       var nTr = this.parentNode.parentNode;
       if (this.src.match('details_close')) {
           // This row is already open - close it 
           this.src = "/Content/images/details_open.png";
           userstable.fnClose(nTr);
       }
       else {
           // Open this row 
           this.src = "/Content/images/details_close.png";
           var companyid = $(this).attr("rel");
           $.get("/office/agent-listing-ajax-handler/?officeId=" + companyid, function (employees) {
               userstable.fnOpen(nTr, employees, 'details');
           });
       }
});

    userstable = $('#userlisting').dataTable({
        "bServerSide": true,
        "scrollX": true,
        "sAjaxSource": "/office/office-listing-ajax-handler",
        "bProcessing": true,
        "order": [
            [1, "asc"]
        ],
        "aoColumns": [
            
           {
               "bVisible": true,
               "bSortable": false,
               "bSearchable": false,
               render: function (oObj) {
                   return '<img src="/Content/images/details_open.png" alt="expand/collapse" rel="' + oObj + '"/>';
               }
           },
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
                 return "<a class=\"btn btn-block btn-primary\" href=\"javascript:ManageOffice('" + full[0] + "');\">Manage</a>";
             }
         }
         ,
         {
             "bSortable": false,
             "bSearchable": false,
             "mRender": function (data, type, full) {
                 return "<a class=\"btn btn-block btn-danger\" href=\"javascript:DeleteOffice('" + full[0] + "');\">Delete</a>";
             }
         }
        ]
    });

}

function ManageOffice(uniqueid) {
    window.location.href = "/office/manage/" + uniqueid;
}

function DeleteOffice(uniqueid) {
    $.ajax({
        url: "/office/delete-office?officeId=" + uniqueid,
        type: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data) {
                $("#sucessalertdelete").show();
                $('#sucessalertdelete').delay(5000).fadeOut(400);
                var userlisting = $("#userlisting").dataTable({ bRetrieve: true });
                userlisting.fnDraw(true);
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

