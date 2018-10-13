using HealthSolution.Dal;
using HealthSolution.Filters;
using HealthSolution.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class ReportController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: Report
        public ActionResult Index()
        {
            return View("Finances");
        }

        public ActionResult Finances()
        {
            return View(new List<FinancaViewModel>());
        }

        [HttpPost]
        public ActionResult Finances(string dataInicio, string dataFim)
        {
            var consultas = db.Consultas.Include(x => x.Especialidade).Include(x => x.Especialista).
            Include(x => x.Paciente).ToList();
            var procedimentos = db.Intervencoes.Include(x => x.Procedimento).Include(x => x.Paciente)
               .Include(x => x.Especialista).ToList();
            var finances = new List<FinancaViewModel>();

            if (!string.IsNullOrEmpty(dataInicio))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(dataInicio, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date >= lvDateTime).ToList();
                    procedimentos = procedimentos.Where(x => x.Date >= lvDateTime).ToList();
                    ViewBag.dataInicio = dataInicio;
                }
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(dataFim, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date <= lvDateTime).ToList();
                    procedimentos = procedimentos.Where(x => x.Date <= lvDateTime).ToList();
                    ViewBag.dataInicio = dataInicio;
                }               
            }

            consultas.ForEach(x =>
            {
                var paymentWay = db.PagamentosConsultas.Where(y => y.ConsultaId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                finances.Add(new FinancaViewModel()
                {
                    Tipo = "Consulta",
                    Data = x.Date,
                    Especialidade = x.Especialidade.Nome,
                    Doutor = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    FormaPagamento = paymentWay != null ? paymentWay.FormaPagamento.Nome : "-",
                    ValorPago = x.ValorPago,
                    Observacao = x.Observacao
                });
            });

            procedimentos.ForEach(x =>
            {
                var payment = db.PagamentosProcedimentos.Where(y => y.IntervencaoId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                finances.Add(new FinancaViewModel()
                {               
                    Tipo = "Procedimento",
                    Data = x.Date,
                    Especialidade = x.Procedimento.Nome,
                    Doutor = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    FormaPagamento = payment != null ? payment.FormaPagamento.Nome : "-",
                    ValorPago = x.ValorPago,
                    Observacao = x.Observacao
                });
            });

            finances = finances.OrderByDescending(x => x.Data).ToList();

            return View(finances);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}