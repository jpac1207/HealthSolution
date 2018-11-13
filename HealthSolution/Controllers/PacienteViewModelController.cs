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

        [MedicalFilter]
        public ActionResult Prontuario(int? id, string doutor, string procedimento, string data)
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

            if (!string.IsNullOrEmpty(doutor))
            {
                consultas = consultas.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor.ToUpper())).ToList();
                procedimentos = procedimentos.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor.ToUpper())).ToList();
                ViewBag.doutor = doutor;
            }
            if (!string.IsNullOrEmpty(procedimento))
            {                
                procedimentos = procedimentos.Where(x => x.Procedimento.Nome.ToUpper().Contains(procedimento.ToUpper())).ToList();
                consultas = new List<Consulta>();
                ViewBag.procedimento = procedimento;
            }
            if (!string.IsNullOrEmpty(data))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(data, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date == lvDateTime).ToList();
                    procedimentos = procedimentos.Where(x => x.Date == lvDateTime).ToList();
                    ViewBag.data = data;
                }
            }

            consultas.ForEach(x =>
            {
                var paymentWay = db.PagamentosConsultas.Where(y => y.ConsultaId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                prontuarios.Add(new ProntuarioViewModel()
                {
                    Id = x.Id,
                    Tipo = "Consulta",
                    Date = x.Date,
                    Especialidade = x.Especialidade.Nome,
                    NomeEspecialista = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    Hora = x.Hora,
                    Minuto = x.Minuto,
                    FormaPagamento = paymentWay != null ? paymentWay.FormaPagamento.Nome : "-",
                    AnotacaoEspecialista = x.AnotacaoEspecialista,
                    Observacao = x.Observacao,
                    Medicamentos = x.Medicamentos

                });
            });

            procedimentos.ForEach(x =>
            {
                var payment = db.PagamentosProcedimentos.Where(y => y.IntervencaoId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

                prontuarios.Add(new ProntuarioViewModel()
                {
                    Id = x.Id,
                    Tipo = "Procedimento",
                    Date = x.Date,
                    Especialidade = x.Procedimento.Nome,
                    NomeEspecialista = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    Hora = x.Hora,
                    Minuto = x.Minuto,
                    FormaPagamento = payment != null ? payment.FormaPagamento.Nome : "-",
                    AnotacaoEspecialista = x.AnotacaoEspecialista,
                    Observacao = x.Observacao,
                    Medicamentos = x.Medicamentos
                });
            });

            prontuarios = prontuarios.OrderByDescending(x => x.Date).ToList();

            return View(prontuarios);
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

        [HttpPost]
        public ActionResult GetAniversariantes()
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            var aniversariantes = db.Pacientes.Where(x => x.DataNascimento.Day == day &&
            x.DataNascimento.Month == month).Include(x => x.Telefone).ToList();
            return Json(aniversariantes);
        }

        [HttpPost]
        public ActionResult GetDetailsProntuario(string tipo, int id)
        {

            var prontuario = new ProntuarioViewModel();
            if (tipo.Equals("Consulta"))
            {
                var elemento = db.Consultas.Where(x => x.Id == id).Include(x => x.Paciente).
                                Include(x => x.Especialista).Include(x => x.Especialidade).FirstOrDefault();
                prontuario.Date = elemento.Date;
                prontuario.Especialidade = elemento.Especialidade.Nome;
                prontuario.NomeEspecialista = elemento.Especialista.Nome;
                prontuario.Observacao = elemento.Observacao;
                prontuario.NomePaciente = elemento.Paciente.Nome;
                prontuario.AnotacaoEspecialista = elemento.AnotacaoEspecialista;
                prontuario.Minuto = elemento.Minuto;
                prontuario.Hora = elemento.Hora;
                prontuario.Tipo = "Consulta";
                prontuario.Medicamentos = elemento.Medicamentos;

                List<AtendimentoArquivo> arquivos = db.AtendimentoArquivo.Where(x => x.AtendimentoId == id).
                                                    Where(x => x.Tipo == tipo).Include(x => x.Arquivo).ToList();
                prontuario.Arquivos = arquivos;

            }

            if (tipo.Equals("Procedimento"))
            {
                var elemento = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Paciente).Include(x => x.Especialista).Include(x => x.Procedimento).FirstOrDefault();
                prontuario.Date = elemento.Date;
                prontuario.Especialidade = elemento.Procedimento.Nome;
                prontuario.NomeEspecialista = elemento.Especialista.Nome;
                prontuario.Observacao = elemento.Observacao;
                prontuario.NomePaciente = elemento.Paciente.Nome;
                prontuario.AnotacaoEspecialista = elemento.AnotacaoEspecialista;
                prontuario.Tipo = "Procedimento";
                prontuario.Medicamentos = elemento.Medicamentos;

                List<AtendimentoArquivo> arquivos = db.AtendimentoArquivo.Where(x => x.AtendimentoId == id).Where(x => x.Tipo == tipo).Include(x => x.Arquivo).ToList();
                prontuario.Arquivos = arquivos;
            }

            return Json(prontuario);
        }

        [HttpPost]
        public ActionResult SalvarArquivos(int id, string tipo)
        {
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    Arquivo arquivo = new Arquivo();

                    if (file != null)
                    {
                        var path = Path.Combine(@Server.MapPath(@"~\Files\Clientes"),
                                   string.Format("c_{0}_{1}", id, file.FileName));
                        if (!System.IO.File.Exists(path))
                        {
                            file.SaveAs(path);
                            arquivo.Path = path;
                            arquivo.OriginalName = file.FileName;
                            db.Arquivos.Add(arquivo);
                            db.SaveChanges();

                            AtendimentoArquivo atendimentoarquivo = new AtendimentoArquivo();
                            atendimentoarquivo.AtendimentoId = id;
                            atendimentoarquivo.Tipo = tipo;
                            atendimentoarquivo.ArquivoId = arquivo.Id;
                            db.AtendimentoArquivo.Add(atendimentoarquivo);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.StackTrace);
                return Json("Erro ao salvar arquivos!");
            }
            return Json("Arquivos salvos com sucesso!");
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
