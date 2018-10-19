using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbarquivo")]
    public class Arquivo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string OriginalName { get; set; }
    }
}