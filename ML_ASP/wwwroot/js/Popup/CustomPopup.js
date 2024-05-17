document.addEventListener('DOMContentLoaded', function () {
    var popup = document.getElementById('popup');
    var popup2 = document.getElementById('popup2');
    var popup3 = document.getElementById('popup3');

    var addDocumentButton = document.getElementById('addDocumentButton');
    var addDocumentButton2 = document.getElementById('addDocumentButton2');
    var addDocumentButton3 = document.getElementById('addDocumentButton3');

    var closeButton = document.querySelector('.close-btn');
    var closeButton2 = document.querySelector('.close-btn2');
    var closeButton3 = document.querySelector('.close-btn3');

    var saveButton = document.getElementById('saveButton');
    var saveButton2 = document.getElementById('saveButton2');
    var saveButton3 = document.getElementById('saveButton3');


    function showPopup() {
        popup.style.display = 'flex';
    }
    function hidePopup() {
        popup.style.display = 'none';
    }
    //--2nd
    function showPopup2() {
        popup2.style.display = 'flex';
    }
    function hidePopup2() {
        popup2.style.display = 'none';
    }
    //--3rd
    function showPopup3() {
        popup3.style.display = 'flex';
    }
    function hidePopup3() {
        popup3.style.display = 'none';
    }

    addDocumentButton.addEventListener('click', showPopup);
    addDocumentButton2.addEventListener('click', showPopup2);
    addDocumentButton3.addEventListener('click', showPopup3);

    closeButton.addEventListener('click', hidePopup);
    closeButton2.addEventListener('click', hidePopup2);
    closeButton3.addEventListener('click', hidePopup3);

    saveButton.addEventListener('click', function () {
        // Add your save logic here
        hidePopup();
    });
    saveButton2.addEventListener('click', function () {
        // Add your save logic here
        hidePopup2();
    });
    saveButton3.addEventListener('click', function () {
        // Add your save logic here
        hidePopup3();
    });
});
