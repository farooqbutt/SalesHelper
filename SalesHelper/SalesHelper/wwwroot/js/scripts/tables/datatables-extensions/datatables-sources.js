/*=========================================================================================
    File Name: datatables-sources.js
    Description: Sources Datatable
    ----------------------------------------------------------------------------------------
    Item Name: Stack - Responsive Admin Theme
    Author: Pixinvent
    Author URL: hhttp://www.themeforest.net/user/pixinvent
==========================================================================================*/

$(document).ready(function () {

    /***************************************
    *       HTML (DOM) sourced data        *
    ****************************************/

    $('.sourced-data').DataTable({
        "ajax": {
            "url": "/Home/Business",
            "dataSrc": "data" // Assuming the data array is under the 'data' key in the response
        },
        "columns": [
            {
                "data": null, "render": function (_data, _type, _row, meta) {
                    // Use meta.row + 1 to display a sequential number
                    return meta.row + 1;
                }, "width": "2%", "orderable": false, "className": "text-center"
            },
            { "data": "name" }, // Business Name column
            {
                "data": null, "render": function (_data, _type, _row, _meta) {
                    // Create actions buttons or links here (e.g., edit, delete)
                    return '<button class="btn btn-sm btn-primary">Edit</button> ' +
                        '<button class="btn btn-sm btn-danger"> Delete</button> ';
                }, "width": "10%", "orderable": false, "className": "text-center"
            }
        ]
    });
});