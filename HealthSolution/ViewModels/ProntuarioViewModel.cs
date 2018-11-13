using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HealthSolution.Models;

namespace HealthSolution.ViewModels
{
    public class ProntuarioViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string NomePaciente { get; set; }
        public string NomeEspecialista { get; set; }
        public DateTime Date { get; set; }
        public string Especialidade { get; set; }
        public string Observacao { get; set; }
        public string FormaPagamento { get; set; }
        public string AnotacaoEspecialista { get; set; }
        public string Medicamentos { get; set; }
        public int Hora { get; set; }
        public int Minuto { get; set; }
        public Boolean AtendimentoRealizado { get; set; }
        public List<AtendimentoArquivo> Arquivos { get; set; }
    }
}