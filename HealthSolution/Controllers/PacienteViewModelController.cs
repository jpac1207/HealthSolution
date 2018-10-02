using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using HealthSolution.Dal;
using HealthSolution.ViewModels;
using HealthSolution.Models;
using System.Web.ModelBinding;
using HealthSolution.Extensions;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace HealthSolution.Controllers
{
    public class PacienteViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: PacienteViewModel
        public ActionResult Index([Form] QueryOptions queryOptions, string nome, string cpf)
        {
            var pacientesViewModel = new List<PacienteViewModel>();
            var pacientes = new List<Paciente>();

            if (!string.IsNullOrEmpty(nome))
            {
                pacientes = db.Pacientes.Where(x => x.Nome.Contains(nome)).ToList();
                ViewBag.nome = nome;
            }
            else
            {
                pacientes = db.Pacientes.ToList();
            }

            if (!string.IsNullOrEmpty(cpf))
            {
                pacientes = pacientes.Where(x => x.Cpf.Equals(cpf)).ToList();
                ViewBag.cpf = cpf;
            }

            queryOptions.SortOrder = SortOrder.DESC;
            var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
            queryOptions.TotalPages = (int)Math.Ceiling((double)pacientes.Count() / queryOptions.PageSize);
            ViewBag.QueryOptions = queryOptions;

            pacientes = pacientes.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

            pacientes.ForEach(x =>
            {
                PacienteViewModel pacienteViewModel = GetPacienteViewModel(x);
                pacientesViewModel.Add(pacienteViewModel);
            });

            return View(pacientesViewModel);
        }

        private PacienteViewModel GetPacienteViewModel(Paciente x)
        {
            var myPhone = db.Telefones.Where(y => y.Id == x.TelefoneId).FirstOrDefault();
            var myAddress = db.Enderecos.Where(y => y.Id == x.EnderecoId).FirstOrDefault();

            var pacienteViewModel = new PacienteViewModel();
            pacienteViewModel.Id = x.Id;
            pacienteViewModel.Nome = x.Nome;
            pacienteViewModel.Cpf = x.Cpf;
            pacienteViewModel.DataNascimento = x.DataNascimento;
            pacienteViewModel.DataCadastro = x.DataCadastro;
            pacienteViewModel.ComoConheceu = x.ComoConheceu;

            if (myPhone != null)
            {
                pacienteViewModel.NumeroTelefone = myPhone.Numero;
            }

            if (myAddress != null)
            {
                pacienteViewModel.Cidade = myAddress.Cidade;
                pacienteViewModel.Bairro = myAddress.Bairro;
                pacienteViewModel.Rua = myAddress.Rua;
                pacienteViewModel.NumeroResidencia = myAddress.Numero;
            }

            return pacienteViewModel;
        }

        // GET: PacienteViewModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PacienteViewModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Cpf,DataNascimento,DataCadastro,ComoConheceu,Cidade,Bairro,Rua,NumeroResidencia,NumeroTelefone")] PacienteViewModel pacienteViewModel)
        {

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var endereco = new Endereco();
                        endereco.Cidade = pacienteViewModel.Cidade;
                        endereco.Bairro = pacienteViewModel.Bairro;
                        endereco.Rua = pacienteViewModel.Rua;
                        endereco.Numero = pacienteViewModel.NumeroResidencia;
                        db.Enderecos.Add(endereco);
                        db.SaveChanges();


                        var telefone = new Telefone();
                        telefone.Numero = pacienteViewModel.NumeroTelefone;
                        db.Telefones.Add(telefone);

                        var paciente = new Paciente();
                        paciente.Nome = pacienteViewModel.Nome;
                        paciente.Cpf = pacienteViewModel.Cpf;
                        paciente.DataNascimento = pacienteViewModel.DataNascimento;
                        paciente.DataCadastro = pacienteViewModel.DataCadastro;
                        paciente.ComoConheceu = pacienteViewModel.ComoConheceu;
                        paciente.EnderecoId = endereco.Id;
                        paciente.TelefoneId = telefone.Id;
                        db.Pacientes.Add(paciente);

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

            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // POST: PacienteViewModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Cpf,DataNascimento,DataCadastro,ComoConheceu,Cidade,Bairro,Rua,NumeroResidencia,NumeroTelefone")] PacienteViewModel pacienteViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var paciente = db.Pacientes.Where(x => x.Id == pacienteViewModel.Id).FirstOrDefault();

                        if (paciente != null)
                        {
                            paciente.Nome = pacienteViewModel.Nome;
                            paciente.Cpf = pacienteViewModel.Cpf;
                            paciente.DataNascimento = pacienteViewModel.DataNascimento;
                            paciente.DataCadastro = pacienteViewModel.DataCadastro;
                            paciente.ComoConheceu = pacienteViewModel.ComoConheceu;

                            var endereco = db.Enderecos.Where(x => x.Id == paciente.EnderecoId).FirstOrDefault();

                            if (endereco != null)
                            {
                                endereco.Cidade = pacienteViewModel.Cidade;
                                endereco.Bairro = pacienteViewModel.Bairro;
                                endereco.Rua = pacienteViewModel.Rua;
                                endereco.Numero = pacienteViewModel.NumeroResidencia;
                            }

                            var telefone = db.Telefones.Where(x => x.Id == paciente.TelefoneId).FirstOrDefault();

                            if (telefone != null)
                            {
                                telefone.Numero = pacienteViewModel.NumeroTelefone;
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
            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // POST: PacienteViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var paciente = db.Pacientes.Where(x => x.Id == id).FirstOrDefault();

                    if (paciente != null)
                    {
                        var endereco = db.Enderecos.Where(x => x.Id == paciente.EnderecoId).FirstOrDefault();

                        var telefone = db.Telefones.Where(x => x.Id == paciente.TelefoneId).FirstOrDefault();

                        db.Pacientes.Remove(paciente);

                        if (telefone != null)
                        {
                            db.Telefones.Remove(telefone);
                        }

                        if (endereco != null)
                        {
                            db.Enderecos.Remove(endereco);
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

        public ActionResult Prontuario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var pacient = db.Pacientes.Where(x => x.Id == id).FirstOrDefault();

            if (pacient == null)
            {
                return HttpNotFound();
            }

            var consultas = db.Consultas.Where(x => x.PacienteId == id)
                .Include(x => x.Especialidade).Include(x => x.Especialista).
                Include(x => x.Paciente).ToList();
            var procedimentos = db.Intervencoes.Where(x => x.PacienteId == id)
                .Include(x => x.Procedimento).Include(x => x.Paciente)
                .Include(x => x.Especialista).ToList();
            var prontuarios = new List<ProntuarioViewModel>();

            consultas.ForEach(x =>
            {
                var paymentWay = db.PagamentosConsultas.Where(y => y.ConsultaId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                prontuarios.Add(new ProntuarioViewModel()
                {
                    Tipo = "Consulta",
                    Date = x.Date,
                    Especialidade = x.Especialidade.Nome,
                    NomeEspecialista = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    FormaPagamento = paymentWay != null ? paymentWay.FormaPagamento.Nome : "-",
                    Observacao = x.Observacao
                });
            });

            procedimentos.ForEach(x =>
            {
                var payment = db.PagamentosProcedimentos.Where(y => y.IntervencaoId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                prontuarios.Add(new ProntuarioViewModel()
                {
                    Tipo = "Procedimento",
                    Date = x.Date,
                    Especialidade = x.Procedimento.Nome,
                    NomeEspecialista = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    FormaPagamento = payment != null ? payment.FormaPagamento.Nome : "-",
                    Observacao = x.Observacao
                });
            });

            prontuarios = prontuarios.OrderByDescending(x => x.Date).ToList();

            return View(prontuarios);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult GetPacienteByCPF(string cpf)
        {
            Paciente paciente = null;
            if (!string.IsNullOrEmpty(cpf))
            {
                paciente = db.Pacientes.Where(x => x.Cpf == cpf).
                    Include(x => x.Telefone).Include(x => x.Endereco).FirstOrDefault();
            }

            if (paciente == null)
                paciente = new Paciente();

            return Json(paciente);
        }

        public ActionResult Export([Form] QueryOptions queryOptions, string nome, string cpf)
        {
            try
            {
                var pacientesViewModel = new List<PacienteViewModel>();
                var pacientes = new List<Paciente>();

                if (!string.IsNullOrEmpty(nome))
                {
                    pacientes = db.Pacientes.Where(x => x.Nome.Contains(nome)).ToList();
                    ViewBag.nome = nome;
                }
                else
                {
                    pacientes = db.Pacientes.ToList();
                }

                if (!string.IsNullOrEmpty(cpf))
                {
                    pacientes = pacientes.Where(x => x.Cpf.Equals(cpf)).ToList();
                    ViewBag.cpf = cpf;
                }

                queryOptions.SortOrder = SortOrder.DESC;
                var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
                queryOptions.TotalPages = (int)Math.Ceiling((double)pacientes.Count() / queryOptions.PageSize);
                ViewBag.QueryOptions = queryOptions;

                pacientes = pacientes.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

                pacientes.ForEach(x =>
                {
                    PacienteViewModel pacienteViewModel = GetPacienteViewModel(x);
                    pacientesViewModel.Add(pacienteViewModel);
                });

                DataTable dt = Utility.ExportListToDataTable(pacientesViewModel);
                var gridView = new GridView();
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                string fileName = "Export_Pacientes_" + DateTime.Now.ToString("dd.MM.yyyy") + ".xls";

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

            return Index(queryOptions, nome, cpf);
        }

    }
}
