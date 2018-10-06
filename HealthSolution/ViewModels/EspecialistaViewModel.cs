using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class EspecialistaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Crm { get; set; }
        public DateTime DataNascimento { get; set; }
        public string ConselhoUF { get; set; }
        public string Email { get; set; }
        
        public int EspecialidadeId { get; set; }
        public string Especialidade { get; set; }

        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Rua { get; set; }
        public string Numero { get; set; }

        public string Telefone { get; set; }

        public bool AtendeSegunda { get; set; }
        public bool AtendeTerca { get; set; }
        public bool AtendeQuarta { get; set; }
        public bool AtendeQuinta { get; set; }
        public bool AtendeSexta { get; set; }
        public bool AtendeSabado { get; set; }
        public bool AtendeDomingo { get; set; }

        public int HoraInicial { get; set; }
        public int MinutoInicial { get; set; }
        public int HoraFinal { get; set; }
        public int MinutoFinal { get; set; }
    }
}