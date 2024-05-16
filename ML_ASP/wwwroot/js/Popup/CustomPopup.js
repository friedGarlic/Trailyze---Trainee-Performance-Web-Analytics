document.addEventListener('DOMContentLoaded', function () {
    var popup = document.getElementById('popup');

    var addDocumentButton = document.getElementById('addDocumentButton');
    var closeButton = document.querySelector('.close-btn');
    var saveButton = document.getElementById('saveButton');

    var addDocumentButton2 = document.getElementById('addDocumentButton2');
    var addDocumentButton3 = document.getElementById('addDocumentButton3');

    function showPopup() {
        popup.style.display = 'flex';
    }

    function hidePopup() {
        popup.style.display = 'none';
    }

    addDocumentButton2.addEventListener('click', showPopup);
    addDocumentButton3.addEventListener('click', showPopup);

    addDocumentButton.addEventListener('click', showPopup);
    closeButton.addEventListener('click', hidePopup);
    saveButton.addEventListener('click', function () {
        // Add your save logic here
        hidePopup();
    });
});
