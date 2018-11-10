using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Dal;
using HealthSolution.ViewModels;
using HealthSolution.Models;
using HealthSolution.Extensions;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using HealthSolution.Filters;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class HomeController : Controller
    {
        private HealthContext db = new HealthContext();
        public ActionResult Index()
        {           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected void Process()
        {
            try
            {
                foreach (string line in System.IO.File.ReadLines(@"C:\Users\asus.PC\Documents\visual studio 2015\Projects\HealthSolution\HealthSolution\Farmacias.csv", System.Text.Encoding.UTF8))
                {
                    string[] values = line.Split(';');
                    Medicamento m = new Medicamento();
                    m.Id = int.Parse(values[1]);
                    m.Nome = values[2];
                    m.Apresentacao = values[11];
                    db.Medicamentos.Add(m);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.StackTrace);
            }
        }
    }
}