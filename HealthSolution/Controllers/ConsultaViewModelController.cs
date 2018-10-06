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
using System.Web.ModelBinding;
using HealthSolution.Extensions;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using HealthSolution.Filters;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class ConsultaViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        private ConsultaViewModel GetConsultaViewModel(Consulta x)
        {
            var paymentWay = db.PagamentosConsultas.Where(y => y.ConsultaId == x.Id).
                Include(y => y.FormaPagamento).FirstOrDefault();

            var consultaViewModel = new ConsultaViewModel();
            consultaViewModel.Date = x.Date;
            consultaViewModel.Hora = x.Hora;
            consultaViewModel.Minuto = x.Minuto;
            consultaViewModel.EspecialidadeId = x.EspecialidadeId;
            consultaViewModel.Especialidade = x.Especialidade;
            consultaViewModel.EspecialistaId = x.EspecialistaId;
            consultaViewModel.Especialista = x.Especialista;
            consultaViewModel.PacienteId = x.PacienteId;
            consultaViewModel.Paciente = x.Paciente;
            consultaViewModel.Id = x.Id;
            consultaViewModel.Observacao = x.Observacao;
            consultaViewModel.ValorPago = x.ValorPago;
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

        // GET: ConsultaViewModel
        public ActionResult Index([Form] QueryOptions queryOptions, string doutor, string paciente, string especialidade, string data)
        {
            var consultasViewModels = new List<ConsultaViewModel>();
            var consultas = db.Consultas.Include(x => x.Especialista).Include(x => x.Especialidade)
                .Include(x => x.Paciente).ToList();

            if (!string.IsNullOrEmpty(doutor))
            {
                consultas = consultas.Where(x => x.Especialista.Nome.Contains(doutor)).ToList();
                ViewBag.doutor = doutor;
            }
            if (!string.IsNullOrEmpty(paciente))
            {
                consultas = consultas.Where(x => x.Paciente.Nome.Contains(paciente)).ToList();
                ViewBag.paciente = paciente;
            }
            if (!string.IsNullOrEmpty(especialidade))
            {
                consultas = consultas.Where(x => x.Especialidade.Nome.Contains(especialidade)).ToList();
                ViewBag.especialidade = especialidade;
            }
            if (!string.IsNullOrEmpty(data))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(data, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date == lvDateTime).ToList();
                    ViewBag.data = data;
                }
            }

            queryOptions.SortOrder = SortOrder.DESC;
            var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
            queryOptions.TotalPages = (int)Math.Ceiling((double)consultas.Count() / queryOptions.PageSize);
            ViewBag.QueryOptions = queryOptions;

            consultas = consultas.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

            consultas.ForEach(x =>
            {
                consultasViewModels.Add(GetConsultaViewModel(x));
            });

            return View(consultasViewModels);
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
        public ActionResult Create([Bind(Include = "Id,Date,Hora,Minuto,EspecialidadeId,EspecialistaId,"+
            "PacienteId,Observacao,ValorPago,FormaPagamentoId")] ConsultaViewModel consultaViewModel,
            [Bind(Include = "Cpf,Nome,DataNascimento")]Paciente formPaciente,
            string cidade, string bairro, string rua, string numero, string telefone)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var paciente = db.Pacientes.Where(x => x.Cpf == formPaciente.Cpf).FirstOrDefault();

                        if (paciente == null)
                        {
                            var lvtelefone = new Telefone();
                            lvtelefone.Numero = telefone;
                            db.Telefones.Add(lvtelefone);
                            db.SaveChanges();

                            var endereco = new Endereco();
                            endereco.Cidade = cidade;
                            endereco.Bairro = bairro;
                            endereco.Rua = rua;
                            endereco.Numero = numero;
                            db.Enderecos.Add(endereco);
                            db.SaveChanges();

                            paciente = new Paciente();
                            paciente.Nome = formPaciente.Nome;
                            paciente.DataNascimento = formPaciente.DataNascimento;
                            paciente.DataCadastro = DateTime.Now;
                            paciente.Cpf = formPaciente.Cpf;
                            paciente.TelefoneId = lvtelefone.Id;
                            paciente.EnderecoId = endereco.Id;
                            db.Pacientes.Add(paciente);
                            db.SaveChanges();
                        }

                        var consulta = new Consulta();
                        consulta.Date = consultaViewModel.Date;
                        consulta.Hora = consultaViewModel.Hora;
                        consulta.Minuto = consultaViewModel.Minuto;
                        consulta.EspecialidadeId = consultaViewModel.EspecialidadeId;
                        consulta.EspecialistaId = consultaViewModel.EspecialistaId;
                        consulta.PacienteId = paciente.Id;
                        consulta.Observacao = consultaViewModel.Observacao;
                        consulta.ValorPago = consultaViewModel.ValorPago;
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
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        DebugLog.Logar(Utility.Details(e));
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
        public ActionResult Edit([Bind(Include = "Id,Date,Hora,Minuto,EspecialidadeId,EspecialistaId,PacienteId,"+
            "Observacao,ValorPago,FormaPagamentoId")] ConsultaViewModel consultaViewModel)
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
                            consulta.Hora = consultaViewModel.Hora;
                            consulta.Minuto = consultaViewModel.Minuto;
                            consulta.EspecialidadeId = consultaViewModel.EspecialidadeId;
                            consulta.EspecialistaId = consultaViewModel.EspecialistaId;
                            consulta.PacienteId = consultaViewModel.PacienteId;
                            consulta.Observacao = consultaViewModel.Observacao;
                            consulta.ValorPago = consultaViewModel.ValorPago;
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

        public ActionResult GetConsultasHoje()
        {
            DateTime data = DateTime.Now.Date;
            List<Consulta> consultas = new List<Consulta>();
            consultas = db.Consultas.Where(x => x.Date == data).Include(x => x.Paciente).Include(x => x.Especialista).Include(x => x.Especialidade).ToList();
            DebugLog.Logar(consultas.Count.ToString());
            return Json(consultas);
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

        public ActionResult Export([Form] QueryOptions queryOptions, string doutor, string paciente, string especialidade, string data)
        {
            try
            {
                var consultasViewModels = new List<ConsultaViewModel>();
                var consultas = db.Consultas.Include(x => x.Especialista).Include(x => x.Especialidade)
                    .Include(x => x.Paciente).ToList();

                if (!string.IsNullOrEmpty(doutor))
                {
                    consultas = consultas.Where(x => x.Especialista.Nome.Contains(doutor)).ToList();
                    ViewBag.doutor = doutor;
                }
                if (!string.IsNullOrEmpty(paciente))
                {
                    consultas = consultas.Where(x => x.Paciente.Nome.Contains(paciente)).ToList();
                    ViewBag.paciente = paciente;
                }
                if (!string.IsNullOrEmpty(especialidade))
                {
                    consultas = consultas.Where(x => x.Especialidade.Nome.Contains(especialidade)).ToList();
                    ViewBag.especialidade = especialidade;
                }
                if (!string.IsNullOrEmpty(data))
                {
                    DateTime lvDateTime = DateTime.MinValue;

                    if (DateTime.TryParse(data, out lvDateTime))
                    {
                        consultas = consultas.Where(x => x.Date == lvDateTime).ToList();
                        ViewBag.data = data;
                    }
                }

                queryOptions.SortOrder = SortOrder.DESC;
                var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
                queryOptions.TotalPages = (int)Math.Ceiling((double)consultas.Count() / queryOptions.PageSize);
                ViewBag.QueryOptions = queryOptions;

                consultas = consultas.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

                consultas.ForEach(x =>
                {
                    consultasViewModels.Add(GetConsultaViewModel(x));
                });
                DataTable dt = Utility.ExportListToDataTable(consultasViewModels);

                int especialidadeCell = 4;
                int especialistaCell = 5;
                int pacienteCell = 6;
                int formaPagamentoCell = 7;

                foreach (DataRow row in dt.Rows)
                {
                    var especialidadeId = Int32.Parse(row[especialidadeCell].ToString());
                    var especialistaId = Int32.Parse(row[especialistaCell].ToString());
                    var pacienteId = Int32.Parse(row[pacienteCell].ToString());
                    var formaPagamentoId = Int32.Parse(row[formaPagamentoCell].ToString());

                    var lvespecialidade = db.Especialidades.Where(x => x.Id == especialidadeId).FirstOrDefault();
                    var lvespecialista = db.Especialistas.Where(x => x.Id == especialistaId).FirstOrDefault();
                    var lvpaciente = db.Pacientes.Where(x => x.Id == pacienteId).FirstOrDefault();
                    var lvformapagamento = db.FormasPagamento.Where(x => x.Id == formaPagamentoId).FirstOrDefault();

                    row[especialidadeCell] = lvespecialidade != null ? lvespecialidade.Nome : "";
                    row[especialistaCell] = lvespecialista != null ? lvespecialista.Nome : "";
                    row[pacienteCell] = lvpaciente != null ? lvpaciente.Nome : "";
                    row[formaPagamentoCell] = lvformapagamento != null ? lvformapagamento.Nome : "";
                }

                var gridView = new GridView();
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                string fileName = "Export_Consultas_" + DateTime.Now.ToString("dd.MM.yyyy") + ".xls";

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

            return Index(queryOptions, doutor, paciente, especialidade, data);
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
