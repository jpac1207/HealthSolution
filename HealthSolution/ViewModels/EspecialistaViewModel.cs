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
    }
}