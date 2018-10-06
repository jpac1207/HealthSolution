var baseUrlPaciente = '../PacienteViewModel/';
var baseUrlConsulta = '../ConsultaViewModel/';
var baseUrlProcedimento = '../Procedimentos/';
var tableWrapper = document.getElementById("tableWrapper");
var tableConsultas = document.getElementById("tableConsultas");
var tableProcedimentos = document.getElementById("tableProcedimentos");
var listConsultas;

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
                table.appendChild(row);
            }

            tableConsultas.appendChild(table);
        }
    }, function (err) { });

    util.doAjax(baseUrlProcedimento + methodProcedimento, null).then(function (data) {
        console.log(data);
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

run();