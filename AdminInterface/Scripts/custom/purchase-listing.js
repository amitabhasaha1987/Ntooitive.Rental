 $(function() {
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
     if (typeof $.cookie('search-data') !== 'undefined')
     {
         var search_value = JSON.parse($.cookie("search-data"));
         var oTable = $('#purchasepropertylisting').dataTable();
         oTable.fnFilter(search_value);
     }
     

 });

 function create_datatable() {
     $('#purchasepropertylisting').dataTable({
         "bServerSide": true,
         "scrollX": true,
         "bDestroy": true,
         "sAjaxSource": "/property/purchase-listing-ajax-handler",
         "bProcessing": true,
         "order": [[2, "asc"]],
         "aoColumns": [
             {
                 "sName": "Image",
                 "bSearchable": false,
                 "bSortable": false,
                 "mRender": function (oObj) {
                     return '<img src=\"' + oObj + '\" style="height: 60px;width: 60px;"/>';
                 }
             }, {
                 "sName": "MlsNumber",
                 "bSearchable": true,
                 "bSortable": false,
             }, {
                 "sName": "Price",
                 "bSearchable": false,
                 "bSortable": true
             }, {
                 "sName": "PropertyType",
                 "bSearchable": true,
                 "bSortable": true,
             }, {
                 "sName": "LivingArea",
                 "bSearchable": false,
                 "bSortable": true,
             }, {
                 "sName": "NoOfBathRooms",
                 "bSortable": false,
                 "bSearchable": false,
             }, {
                 "sName": "NoOfBedRooms",
                 "bSortable": false,
                 "bSearchable": false,
             }, {
                 "sName": "IsNewConstruction",
                 "bSortable": false,
                 "bSearchable": false,
                 render: function (data) {
                     if (data === 'False') {
                         return '<input type="checkbox" disabled>';
                     } else {
                         return '<input type="checkbox" checked disabled>';
                     }
                 }
             }, {
                 "sName": "IsFeatured",
                 "bSortable": false,
                 "bSearchable": false,
                 render: function (data) {
                     if (data === 'False') {
                         return '<input type="checkbox" disabled>';
                     } else {
                         return '<input type="checkbox" checked disabled>';
                     }
                 }
             }, {
                 "sName": "FullStreetAddress",
                 "bSortable": false,
                 "bSearchable": false,
             },
             {
                 "bSortable": false,
                 "bSearchable": false,
                 "mRender": function (data, type, full) {
                     return "<a class=\"btn btn-block btn-primary\" href=\"javascript:ManageProperty('" + full[1] + "');\">Manage</a>";
                 }
             },
             {
                 "bSortable": false,
                 "bSearchable": false,
                 "mRender": function (data, type, full) {
                     return "<a class=\"btn btn-block btn-primary\" href=\"javascript:EditProperty('" + full[1] + "');\">Edit</a>";
                 }
             },
             {
                 "bSortable": false,
                 "bSearchable": false,
                 "mRender": function (data, type, full) {
                     return "<a class=\"btn btn-block btn-danger\" href=\"javascript:DeleteProperty('" + full[1] + "');\">Delete</a>";
                 }
             }
         ]
     });
 }

 function ManageProperty(mlsid) {
     window.location.href = "/property/purchase/edit/" + mlsid;
 }

 function EditProperty(mlsid)
 {
     window.location.href = "/property/purchase/new-Property/" + mlsid;
     
 }

 function DeleteProperty(mlsid)
 {
     $.ajax({
         url: "/Property/delete-PurchaseListing",
         type: "POST",
         contentType: 'application/json',
         dataType: "json",
         data: JSON.stringify({ mlsid: mlsid }),
         success: function (data, textStatus, jqXHR) {
             if (data.success) {
                 
                 var purchasepropertylisting = $("#purchasepropertylisting").dataTable({ bRetrieve: true });
                 purchasepropertylisting.fnDraw(true);
             } else {
                
             }
         },
         error: function (jqXHR, textStatus, errorThrown) {
             
         },
         compete: function () {
             
         }
     });
 }

 