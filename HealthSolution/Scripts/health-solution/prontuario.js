var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";
var fileInput = document.getElementById("fileInput");
var btnAdicionar = document.getElementById("btnAdicionarFile");
var fileList = [];
var tbodyFiles = document.getElementById("tbodyFiles");
var btnSalvar = document.getElementById("btnSalvar");
var btnImprimir = document.getElementById("btnImprimir");
var nomePaciente = document.getElementById("nome-paciente");
var id;
var tipo;
var logo = document.getElementById("logo-clinica");

btnSalvar.addEventListener('click', function (event) {

    event.preventDefault();

    sendFile();

});


createdocument = function () {

    var doc = new jsPDF("portrait", "mm", "a4");


    doc.text(80, 15, "Clínica Santa Rita");
    doc.line(5, 20, 205, 20);
    
    doc.setFillColor(240, 240, 240);
    doc.rect(5, 25, 200, 10, 'FD');
  //  var base64 = imageToBase64(logo.src);
  //  console.log(base64);
    doc.setFontSize(12);
    doc.rect(5, 35, 200, 50);
   // doc.addImage(base64, 'PNG', 15, 40);
    doc.text(10, 33, "Identificação do Paciente");
    doc.text(10, 42, "Nome:" + nomePaciente.innerHTML);
    doc.text(10, 50, "Especialidade:" + $('#lblEspecialidademd').text());
    doc.text(10, 58, "Data Nascimento: 24/10/2012" );


    doc.text(80, 95, "Prescrição Médica");
    doc.line(5, 99, 205, 99);
    doc.rect(5, 105, 200, 120);
    doc.text(10, 110, $("#txtAnotacaoEspecialista").text());

    doc.line(35, 250, 170, 250);
    doc.text(93, 254, "Médico(a)");
   // doc.table(20, 200, [ ])
    var string = doc.output('datauristring');
    var x = window.open();
    x.document.open();
    x.document.location = string;
}

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

function imageToBase64(url) {
   /* var canvas, ctx, dataURL, base64;
    canvas = document.getElementById("myCanvas");
    ctx = canvas.getContext("2d");
    canvas.width = img.width;
    canvas.height = img.height;
    ctx.drawImage(img, 0, 0);
    dataURL = canvas.toDataURL("image/png");
   // base64 = dataURL.replace(/^data:image\/png;base64,/, "");
    return dataURL;*/

   /* var img = new Image();

    img.setAttribute('crossOrigin', 'anonymous');

    img.onload = function () {
        var canvas = document.createElement("canvas");
        canvas.width = this.width;
        canvas.height = this.height;

        var ctx = canvas.getContext("2d");
        ctx.drawImage(this, 0, 0);

        var dataURL = canvas.toDataURL("image/png");
        console.log(dataURL);
        alert(dataURL.replace(/^data:image\/(png|jpg);base64,/, ""));
        return dataURL;
    };

    img.src = url;*/

   
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
    createdocument();
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
            console.log(data);
            modal.find("#lblTipo").text(data.Tipo);
            modal.find("#lblPacientemd").text(data.NomePaciente);
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