var baseUrl = '../EspecialistaViewModel/'; 
var baseUrlPaciente = '../PacienteViewModel/';

var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");
var textPacienteCPF = document.getElementById("CPF");

var fnMudarDropDoutor =  function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

var fnPesquisarCPF = function () {
    fnGetPacienteByCPF(textPacienteCPF.value);
}

function fnGetPacienteByCPF(cpf) {
    var util = new Util();
    var method = 'GetPacienteByCPF';
  
    util.doAjax(baseUrlPaciente + method, "{cpf:'" + cpf + "'}").then(function (data) {
        console.log(data);  
    }, function (err) { console.log(err) });

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