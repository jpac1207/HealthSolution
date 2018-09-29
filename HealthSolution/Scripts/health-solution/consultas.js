var baseUrl = '../EspecialistaViewModel/'; 
var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");

var fnMudarDropDoutor =  function () {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.options[dropEspecialidade.selectedIndex].value);
}

function fnGetDoutorByEspecialidadeId(especialidadeId) {
    var util = new Util();
    var method = 'GetEspecialistasById';
    
    util.doAjax(baseUrl + method, "{especialidadeId:" + especialidadeId + "}").then(function (data) {
        console.log(data);
        while (dropDoutor.options.length > 0)
        {
            dropDoutor.remove(0);
        }

        for (var i = 0; data.length; i++) {
            var opt = document.createElement("option");
            opt.value = data[i].Especialista.Id;
            opt.innerHTML = data[i].Especialista.Nome;
            dropDoutor.appendChild(opt);
        } 
        
    }), function (err) { consoleg.log(err)}; 
}

dropEspecialidade.onchange = fnMudarDropDoutor;