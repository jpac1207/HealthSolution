var baseUrlPaciente = '../PacienteViewModel/';
var tableWrapper = document.getElementById("tableWrapper");

function run() {
    var util = new Util();
    var method = 'GetAniversariantes';

    util.doAjax(baseUrlPaciente + method, null).then(function (data) {

        if (data && data.length > 0) {
            var table = document.createElement("table");
            table.classList.add("table", "table-bordered");
            var headerRow = document.createElement("tr");
            headerRow.classList.add("bg-primary", "text-white");
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
}

run();