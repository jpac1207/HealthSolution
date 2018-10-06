using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbespecialista")]
    public class Especialista
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Crm { get; set; }
        public string ConselhoUF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; }
        public int EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
        public int TelefoneId { get; set; }
        public Telefone Telefone { get; set; }
        public int DiasAtendimentoId { get; set; }
        public DiasAtendimento DiasAtendimento { get; set; }
        public int HoraInicial { get; set; }
        public int MinutoInicial { get; set; }
        public int HoraFinal { get; set; }
        public int MinutoFinal { get; set; }
    }
}