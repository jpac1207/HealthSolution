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
using System.IO;
using System.Web.UI;
using HealthSolution.Filters;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class IntervencaoViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: IntervencaoViewModels
        public ActionResult Index([Form] QueryOptions queryOptions, string doutor, string paciente, string procedimento, string data)
        {
            var intervencaoViewModels = new List<IntervencaoViewModel>();
            var intervencoes = db.Intervencoes.Include(i => i.Especialista).
                Include(i => i.Paciente).Include(i => i.Procedimento).ToList();

            if (!string.IsNullOrEmpty(doutor))
            {
                intervencoes = intervencoes.Where(x => x.Especialista.Nome.Contains(doutor)).ToList();
                ViewBag.doutor = doutor;
            }
            if (!string.IsNullOrEmpty(paciente))
            {
                intervencoes = intervencoes.Where(x => x.Paciente.Nome.Contains(paciente)).ToList();
                ViewBag.paciente = paciente;
            }
            if (!string.IsNullOrEmpty(procedimento))
            {
                intervencoes = intervencoes.Where(x => x.Procedimento.Nome.Contains(procedimento)).ToList();
                ViewBag.procedimento = procedimento;
            }
            if (!string.IsNullOrEmpty(data))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(data, out lvDateTime))
                {
                    intervencoes = intervencoes.Where(x => x.Date == lvDateTime).ToList();
                    ViewBag.data = data;
                }
            }

            queryOptions.SortOrder = SortOrder.DESC;
            var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
            queryOptions.TotalPages = (int)Math.Ceiling((double)intervencoes.Count() / queryOptions.PageSize);
            ViewBag.QueryOptions = queryOptions;

            intervencoes = intervencoes.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

            intervencoes.ForEach(x => intervencaoViewModels.Add(GetIntervencaoViewModel(x)));

            return View(intervencaoViewModels);
        }

        private IntervencaoViewModel GetIntervencaoViewModel(Intervencao x)
        {
            var paymentWay = db.PagamentosProcedimentos.Where(y => y.IntervencaoId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

            var intervencaoViewModel = new IntervencaoViewModel();
            intervencaoViewModel.Date = x.Date;
            intervencaoViewModel.Especialista = x.Especialista;
            intervencaoViewModel.EspecialistaId = x.EspecialistaId;
            intervencaoViewModel.Id = x.Id;
            intervencaoViewModel.Observacao = x.Observacao;
            intervencaoViewModel.Paciente = x.Paciente;
            intervencaoViewModel.PacienteId = x.PacienteId;
            intervencaoViewModel.Procedimento = x.Procedimento;
            intervencaoViewModel.ProcedimentoId = x.ProcedimentoId;

            if (paymentWay != null)
            {
                intervencaoViewModel.FormaPagamentoId = paymentWay.FormaPagamento.Id;
                intervencaoViewModel.FormaPagamento = paymentWay.FormaPagamento;
            }
            else
            {
                intervencaoViewModel.FormaPagamentoId = -1;
                intervencaoViewModel.FormaPagamento = new FormaPagamento()
                {
                    Id = -1,
                    Nome = "-"
                };
            }

            return intervencaoViewModel;
        }

        // GET: IntervencaoViewModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intervencao intervencao = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Especialista)
                .Include(x => x.Paciente).Include(x => x.Procedimento).FirstOrDefault();
            if (intervencao == null)
            {
                return HttpNotFound();
            }
            IntervencaoViewModel intervencaoViewModel = GetIntervencaoViewModel(intervencao);
            if (intervencaoViewModel == null)
            {
                return HttpNotFound();
            }
            return View(intervencaoViewModel);
        }

        // GET: IntervencaoViewModels/Create
        public ActionResult Create()
        {
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome");
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome");
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome");
            ViewBag.ProcedimentoId = new SelectList(db.Procedimentos, "Id", "Nome");
            return View();
        }

        // POST: IntervencaoViewModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,ProcedimentoId,EspecialistaId,PacienteId,Observacao,FormaPagamentoId")] IntervencaoViewModel intervencaoViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var intervencao = new Intervencao();
                        intervencao.Date = intervencaoViewModel.Date;
                        intervencao.ProcedimentoId = intervencaoViewModel.ProcedimentoId;
                        intervencao.EspecialistaId = intervencaoViewModel.EspecialistaId;
                        intervencao.PacienteId = intervencaoViewModel.PacienteId;
                        intervencao.Observacao = intervencaoViewModel.Observacao;
                        db.Intervencoes.Add(intervencao);
                        db.SaveChanges();

                        if (intervencaoViewModel.FormaPagamentoId != -1)
                        {
                            db.PagamentosProcedimentos.Add(new PagamentoProcedimento()
                            {
                                IntervencaoId = intervencao.Id,
                                FormaPagamentoId = intervencaoViewModel.FormaPagamentoId
                            });
                        }

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        DebugLog.Logar(DebugLog.Details(e));
                    }
                }
                return RedirectToAction("Index");
            }

            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", intervencaoViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", intervencaoViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", intervencaoViewModel.PacienteId);
            ViewBag.ProcedimentoId = new SelectList(db.Procedimentos, "Id", "Nome", intervencaoViewModel.ProcedimentoId);
            return View(intervencaoViewModel);
        }

        // GET: IntervencaoViewModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intervencao intervencao = db.Intervencoes.Find(id);
            if (intervencao == null)
            {
                return HttpNotFound();
            }
            IntervencaoViewModel intervencaoViewModel = GetIntervencaoViewModel(intervencao);
            if (intervencaoViewModel == null)
            {
                return HttpNotFound();
            }
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", intervencaoViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", intervencaoViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", intervencaoViewModel.PacienteId);
            ViewBag.ProcedimentoId = new SelectList(db.Procedimentos, "Id", "Nome", intervencaoViewModel.ProcedimentoId);
            return View(intervencaoViewModel);
        }

        // POST: IntervencaoViewModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,ProcedimentoId,EspecialistaId,PacienteId,Observacao,FormaPagamentoId")] IntervencaoViewModel intervencaoViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var intervencao = db.Intervencoes.Where(x => x.Id == intervencaoViewModel.Id).FirstOrDefault();

                        if (intervencao != null)
                        {
                            intervencao.Date = intervencaoViewModel.Date;
                            intervencao.ProcedimentoId = intervencaoViewModel.ProcedimentoId;
                            intervencao.EspecialistaId = intervencaoViewModel.EspecialistaId;
                            intervencao.PacienteId = intervencaoViewModel.PacienteId;
                            intervencao.Observacao = intervencaoViewModel.Observacao;
                            db.SaveChanges();

                            if (intervencaoViewModel.FormaPagamentoId != -1)
                            {
                                var oldPayment = db.PagamentosProcedimentos.Where(x => x.IntervencaoId == intervencao.Id).FirstOrDefault();

                                if (oldPayment != null)
                                    oldPayment.FormaPagamentoId = intervencaoViewModel.FormaPagamentoId;
                                else
                                {
                                    db.PagamentosProcedimentos.Add(new PagamentoProcedimento()
                                    {
                                        IntervencaoId = intervencao.Id,
                                        FormaPagamentoId = intervencaoViewModel.FormaPagamentoId
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
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        DebugLog.Logar(DebugLog.Details(e));
                    }
                }
                return RedirectToAction("Index");
            }
            var paymentWays = db.FormasPagamento.ToList();
            paymentWays.Insert(0, new FormaPagamento() { Id = -1, Nome = "-" });

            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", intervencaoViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", intervencaoViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", intervencaoViewModel.PacienteId);
            ViewBag.ProcedimentoId = new SelectList(db.Procedimentos, "Id", "Nome", intervencaoViewModel.ProcedimentoId);
            return View(intervencaoViewModel);
        }

        // GET: IntervencaoViewModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intervencao intervencao = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Especialista)
                 .Include(x => x.Paciente).Include(x => x.Procedimento).FirstOrDefault();
            if (intervencao == null)
            {
                return HttpNotFound();
            }
            IntervencaoViewModel intervencaoViewModel = GetIntervencaoViewModel(intervencao);
            if (intervencaoViewModel == null)
            {
                return HttpNotFound();
            }
            return View(intervencaoViewModel);
        }

        // POST: IntervencaoViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var intervencao = db.Intervencoes.Where(x => x.Id == id).FirstOrDefault();

                    if (intervencao != null)
                    {
                        db.PagamentosProcedimentos.Where(x => x.IntervencaoId == id).ToList().
                            ForEach(x => db.PagamentosProcedimentos.Remove(x));
                        db.SaveChanges();
                        db.Intervencoes.Remove(intervencao);
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

        public ActionResult Export([Form] QueryOptions queryOptions, string doutor, string paciente, string procedimento, string data)
        {
            try
            {
                var intervencaoViewModels = new List<IntervencaoViewModel>();
                var intervencoes = db.Intervencoes.Include(i => i.Especialista).
                    Include(i => i.Paciente).Include(i => i.Procedimento).ToList();

                if (!string.IsNullOrEmpty(doutor))
                {
                    intervencoes = intervencoes.Where(x => x.Especialista.Nome.Contains(doutor)).ToList();
                    ViewBag.doutor = doutor;
                }
                if (!string.IsNullOrEmpty(paciente))
                {
                    intervencoes = intervencoes.Where(x => x.Paciente.Nome.Contains(paciente)).ToList();
                    ViewBag.paciente = paciente;
                }
                if (!string.IsNullOrEmpty(procedimento))
                {
                    intervencoes = intervencoes.Where(x => x.Procedimento.Nome.Contains(procedimento)).ToList();
                    ViewBag.procedimento = procedimento;
                }
                if (!string.IsNullOrEmpty(data))
                {
                    DateTime lvDateTime = DateTime.MinValue;

                    if (DateTime.TryParse(data, out lvDateTime))
                    {
                        intervencoes = intervencoes.Where(x => x.Date == lvDateTime).ToList();
                        ViewBag.data = data;
                    }
                }

                queryOptions.SortOrder = SortOrder.DESC;
                var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
                queryOptions.TotalPages = (int)Math.Ceiling((double)intervencoes.Count() / queryOptions.PageSize);
                ViewBag.QueryOptions = queryOptions;

                intervencoes = intervencoes.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

                intervencoes.ForEach(x => intervencaoViewModels.Add(GetIntervencaoViewModel(x)));
                DataTable dt = Utility.ExportListToDataTable(intervencaoViewModels);

                int procedimentoCell = 2;
                int especialistaCell = 3;
                int pacienteCell = 4;
                int formaPagamentoCell = 6;

                foreach (DataRow row in dt.Rows)
                {
                    var procedimentoId = Int32.Parse(row[procedimentoCell].ToString());
                    var especialistaId = Int32.Parse(row[especialistaCell].ToString());
                    var pacienteId = Int32.Parse(row[pacienteCell].ToString());
                    var formaPagamentoId = Int32.Parse(row[formaPagamentoCell].ToString());

                    var lvprocedimento = db.Procedimentos.Where(x => x.Id == procedimentoId).FirstOrDefault();
                    var lvespecialista = db.Especialistas.Where(x => x.Id == especialistaId).FirstOrDefault();
                    var lvpaciente = db.Pacientes.Where(x => x.Id == pacienteId).FirstOrDefault();
                    var lvformapagamento = db.FormasPagamento.Where(x => x.Id == formaPagamentoId).FirstOrDefault();

                    row[procedimentoCell] = lvprocedimento != null ? lvprocedimento.Nome : "";                   
                    row[especialistaCell] = lvespecialista != null ? lvespecialista.Nome : "";
                    row[pacienteCell] = lvpaciente != null ? lvpaciente.Nome : "";
                    row[formaPagamentoCell] = lvformapagamento != null ? lvformapagamento.Nome : "";
                }

                var gridView = new GridView();
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                string fileName = "Export_Procedimentos_" + DateTime.Now.ToString("dd.MM.yyyy") + ".xls";

                gridView.DataSource = dt;
                gridView.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                gridView.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
            }

            return Index(queryOptions, doutor, paciente, procedimento, data);
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
