﻿using System;
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
        public int ProcedimentoId  { get; set; }
        public Procedimento Procedimento { get; set; }
        public int EspecialistaId { get; set; }
        public Especialista Especialista { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string Observacao { get; set; }
    }
}