using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class AtendimentoViewModel
    {
        public string NomeEspecialista { get; set; }
        public string AnotacaoEspecialista { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NomePaciente { get; set; }
        public int IdPaciente { get; set; }
        public string AnotacaoMedicamentos { get; set; }
        public string Tipo { get; set; }
        public int IdAtendimento { get; set; }
        public string Especialidade { get; set; }
        public string Observacao { get; set; }
    }
}