var dataTable;

$(document).ready(function () {
    loadDataTable();

    // handle click event on the "Hide" button
    $('#submissionTable').on('click', '.group-btn', function () {
        var folderId = $(this).data('folderid');
        toggleGroupVisibility(folderId);
    });
});

function loadDataTable() {
    dataTable = $('#submissionTable').DataTable({
        "ajax": {
            "url": "/Admin/GetAll", // Endpoint to fetch data from the controller "Admin"  **IMPORTANT
            "type": "GET",
        },
        "columns": [
            { "data": "name" },
            { "data": "date" },
            { "data": "fileName" },
            { "data": "approvalStatus" },
            {
                "data": null,
                "render": function (data, type, row) {
                    return '<button class="view-pdf" data-id="' + row.fileName + '">View</button>';
                }
            }
        ],
        "rowGroup": {
            "dataSrc": "folderId",
            "startRender": function (rows, group) {
                return '<button class="btn btn-primary btn-sm group-btn" data-folderid="' + group + '">Toggle</button> Folder ID: ' + group;
            }
        }
    });

    //TODO its not finding the fileName properly, Try to work this out first
    $('#submissionTable').on('click', '.view-pdf', function () {
        var getId = $(this).data('id');
        window.open('../Admin/ViewPdf2?id=' + getId, '_blank');

    });

}

function toggleGroupVisibility(folderId) {
    var rows = dataTable.rows(function (idx, data, node) {
        return data.folderId === folderId;
    }).nodes();

    $(rows).toggle(); // Toggle visibility of the rows with the specified folderId
}

function sendDataToController(data) {
    $.ajax({
        url: '/Admin/EditProfile',
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (response) {
            console.log("SUCCESS");
        },
        error: function (error) {
            console.error(error);
        }
    });
}