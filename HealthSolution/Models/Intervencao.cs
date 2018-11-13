using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbintervencao")]
    public class Intervencao
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Hora { get; set; }
        public int Minuto { get; set; }
        public int ProcedimentoId  { get; set; }
        public Procedimento Procedimento { get; set; }
        public int EspecialistaId { get; set; }
        public Especialista Especialista { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string Observacao { get; set; }
        public double ValorPago { get; set; }
        public Arquivo Arquivo { get; set; }
        public int ArquivoId { get; set; }
        public string AnotacaoEspecialista { get; set; }
        public string Medicamentos { get; set; }
        public Boolean AtendimentoRealizado { get; set; }
    }
}