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
        var oTable = $('#newhomelisting').dataTable();
        oTable.fnFilter(search_value);
    }
});

function create_datatable() {
    debugger;
    var oTable;

    /*
    $('#newhomelisting').on('click', 'tbody td img', function () {
        debugger;
        var nTr = this.parentNode.parentNode;
        if (this.src.match('details_close')) {
            // This row is already open - close it 
            this.src = "/Content/images/details_open.png";
            oTable.fnClose(nTr);
        }
        else {
            // Open this row 
            this.src = "/Content/images/details_close.png";
            var companyid = $(this).attr("rel");
            $.get("/property/plan-details/" + companyid, function (employees) {
                oTable.fnOpen(nTr, employees, 'details');
            });
        }
});
*/
    debugger;
    /* Initialize table and make first column non-sortable*/
    oTable = $("#newhomelisting").dataTable({
        "bServerSide": true,
        "bProcessing": true,
        "order": [[2, "asc"]],
        "sAjaxSource": '/property/community-listing-ajax-handler',
        "aoColumns": [
                    {
                        "bVisible": false,
                        "bSortable": false,
                        "bSearchable": false,
                        render: function (oObj) {
                            return '<img src="/Content/images/details_open.png" alt="expand/collapse" rel="' + oObj + '"/>';
                        }
                    },
                    {
                        "bSortable": false,
                        "bSearchable": true
                    },
                    {
                        "bSortable": true,
                        "bSearchable": true
                    },
                    {
                        "bSortable": true,
                        "bSearchable": true
                    },
                    {
                        "bSortable": true,
                        "bSearchable": true
                    },
                    {
                        "bSortable": true,
                        "bSearchable": true
                    },
                    {
                        "bSortable": true,
                        "bSearchable": true
                    },
                    {
                        "bSortable": false,
                        "bSearchable": false,
                        "mRender": function (data, type, full) {
                            return "<a class=\"btn btn-block btn-primary\" href=\"javascript:EditCommunity('" + full[0] + "');\">Edit</a>";
                        }
                    },
                    {
                        "bSortable": false,
                        "bSearchable": false,
                        "mRender": function (data, type, full) {
                            return "<a class=\"btn btn-block btn-danger\" href=\"javascript:DeleteCommunity('" + full[0] + "');\">Delete</a>";
                        }
                    }
        ]
    });
}

function EditCommunity(mlsid) {
    debugger;
    window.location.href = "/property/new-community?communityId=" + mlsid;
}

function DeleteCommunity(mlsid) {
    $.ajax({
        url: "/property/delete-community?communityId=" + mlsid,
        type: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data) {
                $("#sucessalert").show();
                $('#sucessalert').delay(5000).fadeOut(400);
                var newhomelisting = $("#newhomelisting").dataTable({ bRetrieve: true });
                newhomelisting.fnDraw(true);
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
