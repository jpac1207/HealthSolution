using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class FinancaViewModel
    {
        public DateTime Data { get; set; }
        public string Tipo { get; set; }
        public string Especialidade { get; set; }
        public string Doutor { get; set; }
        public string NomePaciente { get; set; }
        public double ValorPago { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacao { get; set; }
    }
}