using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Dal;
using HealthSolution.ViewModels;
using HealthSolution.Models;

namespace HealthSolution.Controllers
{
    public class ConsultaViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: ConsultaViewModel
        public ActionResult Index(string doutor, string paciente, string especialidade, string data)
        {
            var consultasViewModels = new List<ConsultaViewModel>();
            var consultas = db.Consultas.Include(x => x.Especialista).Include(x => x.Especialidade)
                .Include(x => x.Paciente).ToList();

            if (!string.IsNullOrEmpty(doutor))
                consultas = consultas.Where(x => x.Especialista.Nome.Contains(doutor)).ToList();
            if (!string.IsNullOrEmpty(paciente))
                consultas = consultas.Where(x => x.Paciente.Nome.Contains(paciente)).ToList();
            if (!string.IsNullOrEmpty(especialidade))
                consultas = consultas.Where(x => x.Especialidade.Nome.Contains(especialidade)).ToList();
            if (!string.IsNullOrEmpty(data))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(data, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date == lvDateTime).ToList();
                }
            }

            consultas.ForEach(x =>
            {
                consultasViewModels.Add(GetConsultaViewModel(x));
            });

            return View(consultasViewModels);
        }
        

        private ConsultaViewModel GetConsultaViewModel(Consulta x)
        {
            var paymentWay = db.PagamentosConsultas.Where(y => y.ConsultaId == x.Id).
                Include(y => y.FormaPagamento).FirstOrDefault();

            var consultaViewModel = new ConsultaViewModel();
            consultaViewModel.Date = x.Date;
            consultaViewModel.EspecialidadeId = x.EspecialidadeId;
            consultaViewModel.Especialidade = x.Especialidade;
            consultaViewModel.EspecialistaId = x.EspecialistaId;
            consultaViewModel.Especialista = x.Especialista;
            consultaViewModel.PacienteId = x.PacienteId;
            consultaViewModel.Paciente = x.Paciente;
            consultaViewModel.Id = x.Id;
            consultaViewModel.Observacao = x.Observacao;
            consultaViewModel.PacienteId = x.PacienteId;

            if (paymentWay != null)
            {
                consultaViewModel.FormaPagamentoId = paymentWay.FormaPagamento.Id;
                consultaViewModel.FormaPagamento = paymentWay.FormaPagamento;
            }
            else
            {
                consultaViewModel.FormaPagamentoId = -1;
                consultaViewModel.FormaPagamento = new FormaPagamento()
                {
                    Id = -1,
                    Nome = "-"
                };
            }

            return consultaViewModel;
        }

        // GET: ConsultaViewModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var consulta = db.Consultas.Where(x => x.Id == id).
                Include(x => x.Especialidade).Include(x => x.Especialista).
                Include(x => x.Paciente).FirstOrDefault();
            if (consulta == null)
            {
                return HttpNotFound();
            }
            ConsultaViewModel consultaViewModel = GetConsultaViewModel(consulta);
            if (consultaViewModel == null)
            {
                return HttpNotFound();
            }
            return View(consultaViewModel);
        }

        // GET: ConsultaViewModel/Create
        public ActionResult Create()
        {
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome");
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome");
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome");
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome");
            return View();
        }

        // POST: ConsultaViewModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,EspecialidadeId,EspecialistaId,PacienteId,Observacao,FormaPagamentoId")] ConsultaViewModel consultaViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var consulta = new Consulta();
                        consulta.Date = consultaViewModel.Date;
                        consulta.EspecialidadeId = consultaViewModel.EspecialidadeId;
                        consulta.EspecialistaId = consultaViewModel.EspecialistaId;
                        consulta.PacienteId = consultaViewModel.PacienteId;
                        consulta.Observacao = consultaViewModel.Observacao;
                        db.Consultas.Add(consulta);
                        db.SaveChanges();

                        if (consultaViewModel.FormaPagamentoId != -1)
                        {
                            db.PagamentosConsultas.Add(new PagamentoConsulta()
                            {
                                ConsultaId = consulta.Id,
                                FormaPagamentoId = consultaViewModel.FormaPagamentoId
                            });
                        }

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                    }
                }
                return RedirectToAction("Index");
            }
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome", consultaViewModel.EspecialidadeId);
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", consultaViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", consultaViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", consultaViewModel.PacienteId);
            return View(consultaViewModel);
        }

        // GET: ConsultaViewModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConsultaViewModel consultaViewModel = GetConsultaViewModel(db.Consultas.Find(id));
            if (consultaViewModel == null)
            {
                return HttpNotFound();
            }
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome", consultaViewModel.EspecialidadeId);
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", consultaViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", consultaViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", consultaViewModel.PacienteId);
            return View(consultaViewModel);
        }

        // POST: ConsultaViewModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,EspecialidadeId,EspecialistaId,PacienteId,Observacao,FormaPagamentoId")] ConsultaViewModel consultaViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var consulta = db.Consultas.Where(x => x.Id == consultaViewModel.Id).FirstOrDefault();

                        if (consulta != null)
                        {
                            consulta.Date = consultaViewModel.Date;
                            consulta.EspecialidadeId = consultaViewModel.EspecialidadeId;
                            consulta.EspecialistaId = consultaViewModel.EspecialistaId;
                            consulta.PacienteId = consultaViewModel.PacienteId;
                            consulta.Observacao = consultaViewModel.Observacao;
                            db.SaveChanges();

                            if (consultaViewModel.FormaPagamentoId != -1)
                            {
                                var oldPayment = db.PagamentosConsultas.Where(x => x.ConsultaId == consulta.Id).FirstOrDefault();

                                if (oldPayment != null)
                                    oldPayment.FormaPagamentoId = consultaViewModel.FormaPagamentoId;
                                else
                                {
                                    db.PagamentosConsultas.Add(new PagamentoConsulta()
                                    {
                                        ConsultaId = consulta.Id,
                                        FormaPagamentoId = consultaViewModel.FormaPagamentoId
                                    });
                                }
                            }

                            db.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                    }
                }
                return RedirectToAction("Index");
            }
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome", consultaViewModel.EspecialidadeId);
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", consultaViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", consultaViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", consultaViewModel.PacienteId);
            return View(consultaViewModel);
        }

        // GET: ConsultaViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var consulta = db.Consultas.Where(x => x.Id == id).Include(x => x.Especialidade).Include(x => x.Especialista).
                Include(x => x.Paciente).FirstOrDefault();
            if (consulta == null)
            {
                return HttpNotFound();
            }
            ConsultaViewModel consultaViewModel = GetConsultaViewModel(consulta);
            if (consultaViewModel == null)
            {
                return HttpNotFound();
            }
            return View(consultaViewModel);
        }

        // POST: ConsultaViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var consulta = db.Consultas.Where(x => x.Id == id).FirstOrDefault();

                    if (consulta != null)
                    {
                        db.PagamentosConsultas.Where(x => x.ConsultaId == id).ToList().
                            ForEach(x => db.PagamentosConsultas.Remove(x));
                        db.SaveChanges();
                        db.Consultas.Remove(consulta);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
            return RedirectToAction("Index");
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
