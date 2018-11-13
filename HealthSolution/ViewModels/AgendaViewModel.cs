using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class AgendaViewModel
    {
        public DateTime Data { get; set; }
        public int Hora { get; set; }
        public int Minuto { get; set; }
        public string Tipo { get; set; }
        public string Especialidade { get; set; }
        public string Doutor { get; set; }
        public string NomePaciente { get; set; }
        public string Observacao { get; set; }
        public int Id { get; set; }
        public Boolean AtendimentoRealizado { get; set; }
    }
}