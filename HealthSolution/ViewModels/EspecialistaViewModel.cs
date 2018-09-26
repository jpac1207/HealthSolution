using HealthSolution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class EspecialistaViewModel
    {
        public int Id { get; set; }
        public int Nome { get; set; }
        public string Crm { get; set; }
        public List<Especialidade> Especialidades { get; set; }
    }
}