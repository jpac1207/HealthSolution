var baseUrl = document.getElementById("ModeloAnamneseController").value + "/";
var modeloAnamneses = document.getElementById("ModeloAnamneses");
var anotacaoEspecialista = document.getElementById("anotacaoEspecialista");

modeloAnamneses.addEventListener('change', function (event) {
    fnGetModeloAnmnese(modeloAnamneses.selectedIndex);
});

function fnGetModeloAnmnese(id) {

    if (id != "-1") {
        var util = new Util();
        var method = 'GetModeloAnamneseById';
        var params = "{id:'" + id + "'}";

        util.doAjax(baseUrl + method, params).then(function (data) {
            anotacaoEspecialista.value = data.Modelo;
        }), function (err) { consoleg.log(err) };
    }
}

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