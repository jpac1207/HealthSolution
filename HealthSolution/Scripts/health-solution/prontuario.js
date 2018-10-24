var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";

$('#modalProntuario').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var recipient = button.data('id') // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)

    var tipo = button.attr("id").split("-")[0];

    var id = button.attr("id").split("-")[1];
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
        }

    }, function (err) { });
}