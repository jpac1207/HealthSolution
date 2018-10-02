var baseUrl = '../EspecialistaViewModel/'; 
var baseUrlPaciente = '../PacienteViewModel/';

var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");

var textPacienteCPF = document.getElementById("cpf");

var fnMudarDropDoutor =  function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

var fnPesquisarCPF = function () {
    fnGetPacienteByCPF(textPacienteCPF.value);
}

function fnGetPacienteByCPF(cpf) {
    var util = new Util();
    var method = 'GetPacienteByCPF';


    var textNomePaciente = document.getElementById("nome");
    var textDataAniversario = document.getElementById("datanascimento");
    var textCidade = document.getElementById("cidade");
    var textBairro = document.getElementById("bairro");
    var textRua = document.getElementById("rua");
    var textResidencia = document.getElementById("numero");
    var textTelefone = document.getElementById("telefoneN");


    textNomePaciente.value = "";
    textDataAniversario.value = "";
    textCidade.value = "";
    textBairro.value = "";
    textRua.value = "";
    textResidencia.value = "";
    textTelefone.value = "";

    util.doAjax(baseUrlPaciente + method, "{cpf:'" + cpf + "'}").then(function (data) { 
        if (data.Cpf != null) {
            var date = new Date(parseInt(data.DataNascimento.replace(/[^0-9 +]/g, '')));
            textNomePaciente.value = data.Nome;
            textDataAniversario.value = date.getDate() + "/" + (date.getMonth() >= 10 ? date.getMonth : '0' + date.getMonth()) + "/" + date.getFullYear();
            textCidade.value = data.Endereco.Cidade;
            textBairro.value = data.Endereco.Bairro;
            textRua.value = data.Endereco.Rua;
            textResidencia.value = data.Endereco.Numero;
            textTelefone.value = data.Telefone.Numero;
        }
        
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