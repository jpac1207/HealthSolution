var baseUrlPaciente = document.getElementById("PacienteViewModelController").value + "/";
var baseUrlConsulta = document.getElementById("ConsultaController").value + "/";
var baseUrlProcedimento = document.getElementById("ProcedimentosController").value + "/";
var tableWrapper = document.getElementById("tableWrapper");
var tableConsultas = document.getElementById("tableConsultas");
var tableProcedimentos = document.getElementById("tableProcedimentos");

var txtConsultas = document.getElementById("consultasdia");
var txtProcedimentos = document.getElementById("procedimentosdia");
var btnProcedimento = document.getElementById("btnProcedimento");
var btnConsulta = document.getElementById("btnConsulta");
var listConsultas;

btnProcedimento.onclick = function () {
    procedimento(txtProcedimentos.value);
}

btnConsulta.onclick = function () {
    consulta(txtConsultas.value);
}

function procedimento(pesquisa) {
    var util = new Util();
    var method = 'GetProcedimentos';
    
    util.doAjax(baseUrlProcedimento + method, "{pesquisar:'" + pesquisa + "'}").then(function (data) {
    
        while (tableProcedimentos.hasChildNodes()) {
            tableProcedimentos.removeChild(tableProcedimentos.lastChild);
        }

        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add('bg-light');
            headerRow.classList.add("bg-primary", "text-dark");
            var nameCell = headerRow.insertCell(0);
            nameCell.innerHTML = "Paciente";
            var nameCell = headerRow.insertCell(1);
            nameCell.innerHTML = "Procedimento";
            var phoneCell = headerRow.insertCell(2);
            phoneCell.innerHTML = "Especialista";
            var phoneCell = headerRow.insertCell(3);
            phoneCell.innerHTML = "Horário";
            table.appendChild(headerRow);

            for (var i = 0; i < data.length; i++) {
                var row = document.createElement("tr");

                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = data[i].Paciente.Nome;


                var lvDateCell = row.insertCell(1);
                var especialidade = data[i].Procedimento.Nome;
                lvDateCell.innerHTML = especialidade;


                var lvPhoneCell = row.insertCell(2);
                lvPhoneCell.innerHTML = data[i].Especialista.Nome;

                var lvPhoneCell = row.insertCell(3);
                lvPhoneCell.innerHTML = util.getHour(parseInt(data[i].Hora), parseInt(data[i].Minuto));
                table.appendChild(row);
            }

            tableProcedimentos.appendChild(table);
        }

    }, function (err) { });

}

function consulta(pesquisa) {

    var util = new Util();
    var method = "GetConsulta"; 
    util.doAjax(baseUrlConsulta + method, "{pesquisar:'" + pesquisa + "'}").then(function (data) {

        while (tableConsultas.hasChildNodes()) {
            tableConsultas.removeChild(tableConsultas.lastChild);
        }

        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add('bg-light');
            headerRow.classList.add("bg-primary", "text-dark");
            var nameCell = headerRow.insertCell(0);
            nameCell.innerHTML = "Paciente";
            var dateCell = headerRow.insertCell(1);
            dateCell.innerHTML = "Especialidade";
            var phoneCell = headerRow.insertCell(2);
            phoneCell.innerHTML = "Especialista";
            var phoneCell = headerRow.insertCell(3);
            phoneCell.innerHTML = "Horário";
            var phoneCell = headerRow.insertCell(4);
            phoneCell.innerHTML = "Status";

            table.appendChild(headerRow);

            for (var i = 0; i < data.length; i++) {
                var row = document.createElement("tr");

                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = data[i].Paciente.Nome;

                var lvDateCell = row.insertCell(1);
                var especialidade = data[i].Especialidade.Nome;
                lvDateCell.innerHTML = especialidade;

                var lvPhoneCell = row.insertCell(2);
                lvPhoneCell.innerHTML = data[i].Especialista.Nome;

                var lvPhoneCell = row.insertCell(3);
                lvPhoneCell.innerHTML = util.getHour(parseInt(data[i].Hora), parseInt(data[i].Minuto));
               
                var lvPhoneCell = row.insertCell(4);
                lvPhoneCell.innerHTML = "ok";
                table.appendChild(row);
            }

            tableConsultas.appendChild(table);
        }

    }, function (err) { });
}

function run() {
    var util = new Util();
    var method = 'GetAniversariantes';
    var methodConsulta = 'GetConsultasHoje';
    var methodProcedimento = 'GetIntervencaoHoje';

    util.doAjax(baseUrlPaciente + method, null).then(function (data) {

        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add('bg-light');
            headerRow.classList.add("bg-primary", "text-dark");
            var nameCell = headerRow.insertCell(0);
            nameCell.innerHTML = "Nome";
            var dateCell = headerRow.insertCell(1);
            dateCell.innerHTML = "Data de Nascimento";
            var phoneCell = headerRow.insertCell(2);
            phoneCell.innerHTML = "Telefone";

            table.appendChild(headerRow);

            for (var i = 0; i < data.length; i++) {
                var row = document.createElement("tr");

                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = data[i].Nome;

                var lvDateCell = row.insertCell(1);
                var date = new Date(parseInt(data[i].DataNascimento.replace(/\/Date\((-?\d+)\)\//, '$1')));
                lvDateCell.innerHTML = util.toDateString(date);

                var lvPhoneCell = row.insertCell(2);
                lvPhoneCell.innerHTML = data[i].Telefone.Numero;

                table.appendChild(row);
            }

            tableWrapper.appendChild(table);
        }
    }, function (err) { });

    util.doAjax(baseUrlConsulta + methodConsulta, null).then(function (data) {
        listConsultas = data;
       
        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add('bg-light');
            headerRow.classList.add("bg-primary", "text-dark");
            var nameCell = headerRow.insertCell(0);
            nameCell.innerHTML = "Paciente";
            var dateCell = headerRow.insertCell(1);
            dateCell.innerHTML = "Especialidade";
            var phoneCell = headerRow.insertCell(2);
            phoneCell.innerHTML = "Especialista";
            var phoneCell = headerRow.insertCell(3);
            phoneCell.innerHTML = "Horário";
            var statusCell = headerRow.insertCell(4);
            statusCell.innerHTML = "Status";

            table.appendChild(headerRow);

            for (var i = 0; i < data.length; i++) {
                var row = document.createElement("tr");

                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = data[i].Paciente.Nome;

                var lvDateCell = row.insertCell(1);
                var especialidade = data[i].Especialidade.Nome;
                lvDateCell.innerHTML = especialidade;

                var lvPhoneCell = row.insertCell(2);
                lvPhoneCell.innerHTML = data[i].Especialista.Nome;

                var lvPhoneCell = row.insertCell(3);
                lvPhoneCell.innerHTML = util.getHour(parseInt(data[i].Hora), parseInt(data[i].Minuto));

                var statusCell = row.insertCell(4);
                statusCell.innerHTML = data[i].AtendimentoRealizado == true ? "Atendimento Realizado" : "Falta Atendimento";

                table.appendChild(row);
            }

            tableConsultas.appendChild(table);
        }
    }, function (err) { });

    util.doAjax(baseUrlProcedimento + methodProcedimento, null).then(function (data) {

        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add('bg-light');
            headerRow.classList.add("bg-primary", "text-dark");
            var nameCell = headerRow.insertCell(0);
            nameCell.innerHTML = "Paciente";
            var nameCell = headerRow.insertCell(1);
            nameCell.innerHTML = "Procedimento";
            var phoneCell = headerRow.insertCell(2);
            phoneCell.innerHTML = "Especialista";
            var phoneCell = headerRow.insertCell(3);
            phoneCell.innerHTML = "Horário";
            var statusCell = headerRow.insertCell(4);
            var statusCell = "Status";
            table.appendChild(headerRow);

            for (var i = 0; i < data.length; i++) {
                var row = document.createElement("tr");

                var lvNameCell = row.insertCell(0);
                lvNameCell.innerHTML = data[i].Paciente.Nome;


                var lvDateCell = row.insertCell(1);
                var especialidade = data[i].Procedimento.Nome;
                lvDateCell.innerHTML = especialidade;


                var lvPhoneCell = row.insertCell(2);
                lvPhoneCell.innerHTML = data[i].Especialista.Nome;

                var lvPhoneCell = row.insertCell(3);
                lvPhoneCell.innerHTML = util.getHour(parseInt(data[i].Hora), parseInt(data[i].Minuto));


                var statusCell = row.insertCell(4);
                statusCell.innerHTML = data[i].AtendimentoRealizado == true ? "Atendimento Realizado" : "Falta Atendimento";
                table.appendChild(row);
            }

            tableProcedimentos.appendChild(table);
        }
    }, function (err) { });
}

run();