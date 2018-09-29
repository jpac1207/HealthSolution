var baseUrl = '../Especialidades/'; 
var dropDoutor = document.getElementById("EspecialistaId");
var dropEspecialidade = document.getElementById("EspecialidadeId");

alert("João pedro é da mamãe!");
function fnMudarDropDoutor() {
    fnGetDoutorByEspecialidadeId(dropEspecialidade.selectedIndex);
}

function fnGetDoutorByEspecialidadeId(especialidadeId) {
    var util = new Util();
    var method = 'GetEspecialistasById';
    util.doAjax(baseUrl + method, "{especialidadeId:" + especialidadeId + "}").then(function (data) {
        alert(data);
        while (dropDoutor.options.length > 0)
        {
            dropDoutor.remove(0);
        }

       /* for (var i = 0; data.length; i++) {
            var opt = document.createElement("option");
            opt.value = data[i][0];
            opt.innerHTML = data[i][1];
            dropDoutor.appendChild(opt);
        } */
        
    }), function (err) { consoleg.log(err)}; 
}

dropEspecialidade.onchange = mudarDropDoutor();