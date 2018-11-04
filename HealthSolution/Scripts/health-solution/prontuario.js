var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";
var fileInput = document.getElementById("fileInput");
var btnAdicionar = document.getElementById("btnAdicionarFile");
var fileList = [];
var tbodyFiles = document.getElementById("tbodyFiles");
var btnSalvar = document.getElementById("btnSalvar");
var btnImprimir = document.getElementById("btnImprimir");
var id;
var tipo;

btnSalvar.addEventListener('click', function (event) {

    event.preventDefault();

    sendFile();

});

sendFile = function () {
    var arquivos = [];
    var formData = new FormData();

    for (var i = 0; i < fileList.length; i++) {
        formData.append("file",fileList[i]);   
    }

    formData.append("id", id);
    formData.append("tipo", tipo);

    var util = new Util();
    var method = 'SalvarArquivos';
    var params = "{ id: '" + id + "', tipo:'" + tipo + "'}";

    util.doAjax(baseUrlPaciente + method, params).then(function (data) {
        console.log(data);
       
    }), function (err) { consoleg.log(err) };
}

btnAdicionar.addEventListener('click', function (event) {

        fileList.push(fileInput.file);
        var row = document.createElement("tr");
        var lvNameCell = row.insertCell(0);
        lvNameCell.innerHTML = "<label class='ml-3 mt-2'>" +  fileInput.files[0].name +  "</label>";

        //var lvNameCell1 = row.insertCell(1);
        //lvNameCell1.innerHTML = " <span class='oi oi-eye mr-1 mt-2' style='font-size:larger; color: dimgrey; cursor: pointer'></span>";

        var lvNameCell1 = row.insertCell(1);
        lvNameCell1 .innerHTML = "<span class='ml-4 oi oi-data-transfer-download mt-2' style='font-size:large; color: dimgrey; cursor: pointer'></span>";
        tbodyFiles.appendChild(row);
    
});

btnImprimir.addEventListener('click', function (event) {
    var someJSONdata = [
        {
            name: 'John Doe',
            email: 'john@doe.com',
            phone: '111-111-1111'
        },
        {
            name: 'Barry Allen',
            email: 'barry@flash.com',
            phone: '222-222-2222'
        },
        {
            name: 'Cool Dude',
            email: 'cool@dude.com',
            phone: '333-333-3333'
        }
    ]

    printJS({
        printable: someJSONdata,
        properties: ['name', 'email', 'phone'],
        type: 'json',
        gridHeaderStyle: 'color: red;  border: 2px solid #3971A5;',
        gridStyle: 'border: 2px solid #3971A5;'
    })
});




$('#modalProntuario').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var recipient = button.data('id') // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)

    tipo = button.attr("id").split("-")[0];

    id = button.attr("id").split("-")[1];
    
    getProntuario(tipo, id, modal);
})

function getProntuario (tipo, id, modal) {
    var util = new Util();
    var method = 'GetDetailsProntuario';

    util.doAjax(baseUrlPaciente + method, "{tipo:'" + tipo + "', id:'" + id + "'}").then(function (data) {
        
        if (data) {
            modal.find("#lblTipo").text(data.Tipo);
            modal.find('#lblEspecialistamd').text(data.NomeEspecialista);
            modal.find('#lblEspecialidademd').text(data.Especialidade);
            var date = new Date(parseInt(data.Date.replace(/\/Date\((-?\d+)\)\//, '$1')));
            modal.find("#lblData").text(util.toDateString(date));
            modal.find("#txtAnotacaoEspecialista").text(data.AnotacaoEspecialista);
            modal.find("#txtObservacao").text(data.Observacao);
            modal.find("#txtMedicamentos").text(data.Medicamentos);
            modal.find("#tbodyFiles").text("");
            
            for (var i = 0; i < data.Arquivos.length; i++) {

                var row = document.createElement("tr");
                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = "<label class='ml-3 mt-2'>" + data.Arquivos[i].Arquivo.OriginalName + "</label>";

                //var lvNameCell1 = row.insertCell(1);
                //lvNameCell1.innerHTML = " <span class='oi oi-eye mr-1 mt-2' style='font-size:larger; color: dimgrey; cursor: pointer'></span>";

                var lvNameCell1 = row.insertCell(1);
                lvNameCell1.innerHTML = "<span class='ml-4 oi oi-data-transfer-download mt-2' style='font-size:large; color: dimgrey; cursor: pointer'></span>";
                tbodyFiles.appendChild(row);
            }
        }

    }, function (err) { });
}