var counter = 1;
setInterval(function () {
    var radioElement = document.getElementById('radio' + counter);

    if (radioElement) {
        radioElement.checked = true;
        counter++;
    } else {
        counter = 1;
    }
}, 4000);

/*
document.getElementById('next').onclick = function () {
    let lists = document.querySelectorAll('.item');
    document.getElementById('slider').appendChild(lists[1]);
}

document.getElementById('prev').onclick = function () {
    let lists = document.querySelectorAll('.item');
    document.getElementById('slider').prepend(lists[lists.length - 1]);
}
*/