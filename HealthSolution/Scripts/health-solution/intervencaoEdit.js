var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";
var baseUrlEspecialista = document.getElementById("EspecialistaViewModelController").value + "/";

var textPacienteCPF = document.getElementById("Cpf");
var dropDoutor = document.getElementById("EspecialistaId");
var textDate = document.getElementById("Date");
var textHora = document.getElementById("Hora");
var textMinuto = document.getElementById("Minuto");

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

    util.doAjax(baseUrlEspecialista + method, params).then(function (data) {
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

textDate.onchange = fnVerificarHorario;
textHora.onchange = fnVerificarHorario;
textMinuto.onchange = fnVerificarHorario;
dropDoutor.onchange = fnVerificarHorario;