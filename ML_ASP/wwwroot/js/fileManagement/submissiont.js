var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/FileManagement/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%"},
            { "data": "approvalStatus", "width": "15%" },
            { "data": "fileName", "width": "15%" }
        ],
        "rowGroup": {
            "dataSrc": "folderId",
            "startRender": function (rows, group) {
                return '<button class="btn btn-primary btn-sm group-btn" data-folderid="' + group + '">Compiled</button> Folder ID: ' + group;
            }
        }
    });
}

