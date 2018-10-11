var baseUrl = document.getElementById("EspecialistaViewModelController").value + "/";
var dropEspecialidade = document.getElementById("EspecialidadeId");
var dropDoutor = document.getElementById("EspecialistaId");
var textDate = document.getElementById("Date");
var textHora = document.getElementById("Hora");
var textMinuto = document.getElementById("Minuto");

var fnMudarDropDoutor = function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

var fnVerificarHorario = function () {

    if (textDate.value && textHora.value && textMinuto.value && dropDoutor) {
        fnVerifyHour(textDate.value, textHora.value, textMinuto.value,
            dropDoutor.options[dropDoutor.selectedIndex].value);
    }
}

function fnVerifyHour(data, hora, minuto, doutorId) {
    var util = new Util();
    var method = 'VerifyDoctorTime';
    var params = "{data:'" + data + "', hora: '" + hora + "', minuto: '" + minuto + "', doutorId:'" + doutorId + "'}";

    util.doAjax(baseUrl + method, params).then(function (data) {
        console.log(data);
        if (data != null) {
            var lvdata = data;
            if (!lvdata[0]) {
                util.sendMessage("alert alert-danger", lvdata[1]);
            }
            else {
                util.sendMessage(" ", " ")
            }
        }

    }, function (err) { consoleg.log(err) });
}

function fnGetDoutorByEspecialidadeId(especialidadeId) {
    var util = new Util();
    var method = 'GetEspecialistasById';

    util.doAjax(baseUrl + method, "{especialidadeId:" + especialidadeId + "}").then(function (data) {

        while (dropDoutor.options.length > 0) {
            dropDoutor.remove(0);
        }

        for (var i = 0; i < data.length; i++) {
            var opt = document.createElement("option");
            opt.value = data[i].Especialista.Id;
            opt.innerHTML = data[i].Especialista.Nome;
            dropDoutor.appendChild(opt);
        }

    }, function (err) { consoleg.log(err) });
}

dropEspecialidade.onchange = fnMudarDropDoutor;
textDate.onchange = fnVerificarHorario;
textHora.onchange = fnVerificarHorario;
textMinuto.onchange = fnVerificarHorario;
dropDoutor.onchange = fnVerificarHorario;