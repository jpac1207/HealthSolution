function Util() { };
Util.prototype.doAjax = function (url, data) {
    var options = {
        url: url,
        headers: {
            Accept: "application/json"
        },
        contentType: "application/json",
        cache: false,
        type: 'POST',
        data: data ? data : null
    };
    return $.ajax(options);
}
Util.prototype.stringToDate = function (inputFormat) {
    var dateParts = inputFormat.split('/');
    var date = new Date([dateParts[2], dateParts[1], dateParts[0]].join('/'));
    return date;
}
Util.prototype.toDateString = function (date) {
    var year = date.getFullYear();
    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}

Util.prototype.getHour = function (hora, minuto) {

    var lvHour = hora < 10 ? ("0" + hora) : hora.toString();
    var lvMinute = minuto < 10 ? ("0" + minuto.toString()) : minuto.toString();
    return (lvHour + ":" + lvMinute);
}

Util.prototype.sendMessage = function(styleClass, message){
    var div = document.getElementById("globalMessage");
    div.setAttribute("class", styleClass);
    div.innerHTML = message;
}

Util.prototype.showModal = function (content) {
    
    var modal = new tingle.modal({
        footer: true,
        stickyFooter: false,
        closeMethods: ['overlay', 'button', 'escape'],
        closeLabel: "Close",
        cssClass: ['custom-class-1', 'custom-class-2'],
        onOpen: function () {
            //console.log('modal open');
        },
        onClose: function () {
            //console.log('modal closed');
        },
        beforeClose: function () {           
            return true; // close the modal           
        }
    });

    // set content
    modal.setContent(content);

    // add a button
    modal.addFooterBtn('OK', 'tingle-btn tingle-btn--primary', function () {
        // here goes some logic
        modal.close();
    });

    //// add another button
    //modal.addFooterBtn('Dangerous action !', 'tingle-btn tingle-btn--danger', function () {
    //    // here goes some logic
    //    modal.close();
    //});

    // open modal
    modal.open();

    //// close modal
    //modal.close();
}
