﻿using System;
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

        public int EspecialidadeId { get; set; }
        public string especialidade { get; set; }
    }
}