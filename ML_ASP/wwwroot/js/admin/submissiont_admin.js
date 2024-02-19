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
			{
				"data": "approvalStatus",
				"render": function (data, type, row) {
					var options = ["Approved", "Declined", "Revised"];

					// i dont understand this anymore sadly it got too complicated
					var selectHtml = '<select name="approvalStatus">';
					for (var i = 0; i < options.length; i++) {
						var isSelected = row.approvalStatus === options[i] ? 'selected="selected"' : '';
						selectHtml += '<option value="' + options[i] + '" ' + isSelected + '>' + options[i] + '</option>';
					}
					selectHtml += '</select>';

					var hiddenInputApproval = '<input type="hidden" name="originalApprovalStatus" value = "' + row.approvalStatus + '" />'; //to pass in controller
					var hiddenInputHtml = '<input type="hidden" name="id" value="' + row.id + '">'; // to pass in controller
					return selectHtml + hiddenInputApproval + hiddenInputHtml;
				}
			},
			{
				"data": null,
				"render": function (data, type, row) {
					return '<button class="view-pdf" data-id="' + row.fileName + '" data-folderid="' + row.folderId + '">View</button>';
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
		var getFolderId = $(this).data('folderid');
		if (getFolderId == null) {
			window.open('../Admin/ViewPdf2?id=' + getId, '_blank');
		}
		else {
			window.open('../Admin/ViewPdf2?id=' + getFolderId + '/' + getId, '_blank');
		}
	});

}

function toggleGroupVisibility(folderId) {
    var rows = dataTable.rows(function (idx, data, node) {
        return data.folderId === folderId;
    }).nodes();

    $(rows).toggle(); // Toggle visibility of the rows with the specified folderId
}

function getSubmissionOptionList(data) {
    $.ajax({
        url: '/Admin/GetOptionList',
        type: 'GET',
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