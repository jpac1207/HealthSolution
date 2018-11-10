var baseUrl = document.getElementById("EspecialistaViewModelController").value + "/";

$(function () {

    var medicamentos = [];
    var method = "GetMedicamentos";
    var util = new Util();

    util.doAjax(baseUrl + method).then(function (data) {
        for (var i = 0; i < data.length; i++) {
            medicamentos.push(data[i]);
        }
    }, function (err) { console.log(err) });

    $("#txtMedicamentos").autocomplete({
        source: medicamentos
    });

    $("#btnAdicionar").click(function () {
        $("#txtmedicamentos").val($("#txtmedicamentos").val() + $("#txtMedicamentos").val() + "\n");
    });
});