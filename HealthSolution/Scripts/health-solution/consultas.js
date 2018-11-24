var baseUrl = document.getElementById("EspecialistaViewModelController").value + "/";
var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";

var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");
var textPacienteCPF = document.getElementById("Cpf");
var textDate = document.getElementById("Date");
var textHora = document.getElementById("Hora");
var textMinuto = document.getElementById("Minuto");
var btnBuscar = document.getElementById("btnBuscar");

var fnMudarDropDoutor = function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

var fnPesquisarCPF = function () {
    fnGetPacienteByCPF(textPacienteCPF.value);
}

var fnVerificarHorario = function () {

    if (textDate.value && textHora.value && textMinuto.value && dropDoutor) {
        fnVerifyHour(textDate.value, textHora.value, textMinuto.value,
            dropDoutor.options[dropDoutor.selectedIndex].value);
    }
}

function fnGetPacienteByCPF(cpf) {
    var util = new Util();
    var method = 'GetPacienteByCPF';

    var textNomePaciente = document.getElementById("Nome");
    var textDataAniversario = document.getElementById("DataNascimento");
    var textCidade = document.getElementById("cidade");
    var textBairro = document.getElementById("bairro");
    var textRua = document.getElementById("rua");
    var textResidencia = document.getElementById("numero");
    var textTelefone = document.getElementById("telefone");

    textNomePaciente.value = "";
    textDataAniversario.value = "";
    textCidade.value = "";
    textBairro.value = "";
    textRua.value = "";
    textResidencia.value = "";
    textTelefone.value = "";

    util.doAjax(baseUrlPaciente + method, "{cpf:'" + cpf + "'}").then(function (data) {
        if (data.Cpf != null) {
            var date = new Date(parseInt(data.DataNascimento.replace(/\/Date\((-?\d+)\)\//, '$1')));
            textNomePaciente.value = data.Nome;
            textDataAniversario.value = util.toDateString(date);
            textCidade.value = data.Endereco.Cidade;
            textBairro.value = data.Endereco.Bairro;
            textRua.value = data.Endereco.Rua;
            textResidencia.value = data.Endereco.Numero;
            textTelefone.value = data.Telefone.Numero;
        }
        else {
            util.showModal("Não foi possível encontrar esse paciente!");
        }

    }, function (err) { consoleg.log(err) });

}

function fnGetPacienteByName(name) {
    var util = new Util();
    var method = 'GetPacienteByName';

    var textNomePaciente = document.getElementById("Nome");
    var textDataAniversario = document.getElementById("DataNascimento");
    var textCidade = document.getElementById("cidade");
    var textBairro = document.getElementById("bairro");
    var textRua = document.getElementById("rua");
    var textResidencia = document.getElementById("numero");
    var textTelefone = document.getElementById("telefone");

    textPacienteCPF.value = "";
    textNomePaciente.value = "";
    textDataAniversario.value = "";
    textCidade.value = "";
    textBairro.value = "";
    textRua.value = "";
    textResidencia.value = "";
    textTelefone.value = "";

    util.doAjax(baseUrlPaciente + method, "{name:'" + name + "'}").then(function (data) {
        if (data.Cpf != null) {
            console.log(data)
            var date = new Date(parseInt(data.DataNascimento.replace(/\/Date\((-?\d+)\)\//, '$1')));
            textNomePaciente.value = data.Nome;
            textDataAniversario.value = util.toDateString(date);
            textCidade.value = data.Endereco.Cidade;
            textBairro.value = data.Endereco.Bairro;
            textRua.value = data.Endereco.Rua;
            textResidencia.value = data.Endereco.Numero;
            textTelefone.value = data.Telefone.Numero;
            textPacienteCPF.value = data.Cpf;
        }
        else {
            util.showModal("Não foi possível encontrar esse paciente!");
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

    }), function (err) { consoleg.log(err) };
}

dropEspecialidade.onchange = fnMudarDropDoutor;
//textPacienteCPF.onchange = fnPesquisarCPF;
textDate.onchange = fnVerificarHorario;
textHora.onchange = fnVerificarHorario;
textMinuto.onchange = fnVerificarHorario;
dropDoutor.onchange = fnVerificarHorario;
btnBuscar.onclick = fnPesquisarCPF;
fnMudarDropDoutor();

$(function () {

    var pacientes = [];
    var method = "GetPacientes";
    var util = new Util();

    util.doAjax(baseUrlPaciente + method).then(function (data) {
        for (var i = 0; i < data.length; i++) {
            pacientes.push(data[i].Nome);
        }
    }, function (err) { console.log(err) });

    $("#Nome").autocomplete({
        source: pacientes
    });

    $("#Nome").change(function () {       
        fnGetPacienteByName($("#Nome").val());
    });
});