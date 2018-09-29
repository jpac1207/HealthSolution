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
