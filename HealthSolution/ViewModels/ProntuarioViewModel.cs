using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class ProntuarioViewModel
    {
        public int id { get; set; }
        public string Tipo { get; set; }
        public string NomePaciente { get; set; }
        public string NomeEspecialista { get; set; }
        public DateTime Date { get; set; }
        public string Especialidade { get; set; }
        public string Observacao { get; set; }
        public string FormaPagamento { get; set; }
    }
}