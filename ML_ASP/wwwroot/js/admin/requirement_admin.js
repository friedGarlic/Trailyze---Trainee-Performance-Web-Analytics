var dataTable;

$(document).ready(function () {
	loadDataTable();
});

function loadDataTable() {
    dataTable = $('#requirementFilesTable').DataTable({
        "ajax": {
            "url": "/Admin/GetAllReqFile", // Endpoint to fetch data from the controller "Admin"  **IMPORTANT
        },
        "columns": [
            { "data": "id", "title": "Account ID" },
            { "data": "fullName", "title": "Full Name" },
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": '<span class="toggle-details">Collapsed</span>',
                "title": "Requirements"
            }
        ],
        "order": [[1, 'asc']]
    });

    $('#requirementFilesTable tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = dataTable.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(this).find('.toggle-details').text('Clicked to Expand');
        } else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
            $(this).find('.toggle-details').text('Clicked to Collapse');
        }
    });
}

// Function to format the details row content
function format(data) {
    var requirements = "";
    data.requirements.forEach(function (requirement) {
        requirements += requirement.fileName + "<br>";
    });
    return '<div>' + requirements + '</div>';
}
