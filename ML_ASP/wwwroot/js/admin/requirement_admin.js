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
			{ "data": "userName"},
			{ "data": "fileName" },
			{ "data": "title" },
			{ "data": "description" },
			{
				"data": "approvalStatus",
				"render": function (data, type, row) {
					var options = ["Pending", "Declined", "Approved"];

					// i dont understand this anymore sadly it got too complicated
					//dont try to do this again, took too much time, not worth
					var selectHtml = '<select name="approvalStatus">';
					for (var i = 0; i < options.length; i++) {
						var isSelected = row.approvalStatus === options[i] ? 'selected="selected"' : '';
						selectHtml += '<option value="' + options[i] + '" ' + isSelected + '>' + options[i] + '</option>';
					}
					selectHtml += '</select>';
					var hiddenInputApproval = '<input type="hidden" name="originalApprovalStatus" value = "' + row.approvalStatus + '" />';
					var hiddenInputHtml = '<input type="hidden" name="id" value="' + row.id + '">'; // to pass in controller
					var hiddentInputUserId = '<input type="hidden" name="userId" value="' + row.userId + '">';
					return selectHtml + hiddenInputHtml + hiddentInputUserId + hiddenInputApproval;
				}
			},
			{
				"data": null,
				"render": function (data, type, row) {
					return '<button class="btn btn-secondary btn-sm view-pdf" data-id="' + row.fileName + '">View</button>';
				}
			}

		]
	});

	$('#requirementFilesTable').on('click', '.view-pdf', function () {
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