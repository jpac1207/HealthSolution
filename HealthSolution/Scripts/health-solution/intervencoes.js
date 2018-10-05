var baseUrlPaciente = '../PacienteViewModel/';
var textPacienteCPF = document.getElementById("Cpf");
var fnPesquisarCPF = function () {
    fnGetPacienteByCPF(textPacienteCPF.value);
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

    }), function (err) { consoleg.log(err) };

}

textPacienteCPF.onchange = fnPesquisarCPF;