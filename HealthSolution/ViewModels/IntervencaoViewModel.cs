using HealthSolution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class IntervencaoViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Hora { get; set; }
        public int Minuto { get; set; }
        public int ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }
        public int EspecialistaId { get; set; }
        public Especialista Especialista { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string Observacao { get; set; }
        public double ValorPago { get; set; }
        public string LinkArquivo { get; set; }

        public int FormaPagamentoId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}