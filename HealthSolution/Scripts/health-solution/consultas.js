var baseUrl = '../EspecialistaViewModel/'; 
var baseUrlPaciente = '../PacienteViewModel/';

var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");

var textPacienteCPF = document.getElementById("Paciente_Cpf");

var fnMudarDropDoutor =  function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

var fnPesquisarCPF = function () {
    fnGetPacienteByCPF(textPacienteCPF.value);
}

function fnGetPacienteByCPF(cpf) {
    var util = new Util();
    var method = 'GetPacienteByCPF';


    var textNomePaciente = document.getElementById("Paciente_Nome");
    var textDataAniversario = document.getElementById("Paciente_DataNascimento");
    var textCidade = document.getElementById("Paciente_Endereco_Cidade");
    var textBairro = document.getElementById("Paciente_Endereco_Bairro");
    var textRua = document.getElementById("Paciente_Endereco_Rua");
    var textResidencia = document.getElementById("Paciente_Endereco_Numero");
    var textTelefone = document.getElementById("Paciente_Telefone_Numero");


    textNomePaciente.value = "";
    textDataAniversario.value = "";
    textCidade.value = "";
    textBairro.value = "";
    textRua.value = "";
    textResidencia.value = "";
    textTelefone.value = "";

    util.doAjax(baseUrlPaciente + method, "{cpf:'" + cpf + "'}").then(function (data) {
        var date = new Date(parseInt(data.DataNascimento.replace(/[^0-9 +]/g, '')));
        textNomePaciente.value = data.Nome;
        textDataAniversario.value = date.getDate() + "/" + (date.getMonth() >= 10 ? date.getMonth: '0' + date.getMonth()) + "/" + date.getFullYear();
        textCidade.value = data.Endereco.Cidade;
        textBairro.value = data.Endereco.Bairro;
        textRua.value = data.Endereco.Rua;
        textResidencia.value = data.Endereco.Numero;
        textTelefone.value = data.Telefone.Numero;
        
    }), function (err) { consoleg.log(err) }; 

}

function fnGetDoutorByEspecialidadeId(especialidadeId) {
    var util = new Util();
    var method = 'GetEspecialistasById';
    
    util.doAjax(baseUrl + method, "{especialidadeId:" + especialidadeId + "}").then(function (data) {
       
        while (dropDoutor.options.length > 0)
        {
            dropDoutor.remove(0);
        }

        for (var i = 0; i < data.length; i++) {
            var opt = document.createElement("option");
            opt.value = data[i].Especialista.Id;
            opt.innerHTML = data[i].Especialista.Nome;
            dropDoutor.appendChild(opt);
        } 
        
    }), function (err) { consoleg.log(err)}; 
}

dropEspecialidade.onchange = fnMudarDropDoutor;
textPacienteCPF.onchange = fnPesquisarCPF;