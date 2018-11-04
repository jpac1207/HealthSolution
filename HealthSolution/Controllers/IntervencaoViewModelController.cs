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

        private IntervencaoViewModel GetIntervencaoViewModel(Intervencao x)
        {
            var paymentWay = db.PagamentosProcedimentos.Where(y => y.IntervencaoId == x.Id)
                .Include(y => y.FormaPagamento).FirstOrDefault();

            var intervencaoViewModel = new IntervencaoViewModel();
            intervencaoViewModel.Date = x.Date;
            intervencaoViewModel.Hora = x.Hora;
            intervencaoViewModel.Minuto = x.Minuto;
            intervencaoViewModel.Especialista = x.Especialista;
            intervencaoViewModel.EspecialistaId = x.EspecialistaId;
            intervencaoViewModel.Id = x.Id;
            intervencaoViewModel.Observacao = x.Observacao;
            intervencaoViewModel.ValorPago = x.ValorPago;
            intervencaoViewModel.Paciente = x.Paciente;
            intervencaoViewModel.PacienteId = x.PacienteId;
            intervencaoViewModel.Procedimento = x.Procedimento;
            intervencaoViewModel.ProcedimentoId = x.ProcedimentoId;
            intervencaoViewModel.LinkArquivo = x.Arquivo.Path;

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

        private List<SelectListItem> GetListHour()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();

            for (int i = 0; i < 24; i++)
            {
                var text = i < 10 ? "0" + i : i.ToString();
                var item = new SelectListItem() { Text = text, Value = i.ToString() };
                lista_UF.Add(item);
            }

            return lista_UF;
        }

        private List<SelectListItem> GetListMinute()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();
            lista_UF.Add(new SelectListItem() { Text = "00", Value = "0" });
            lista_UF.Add(new SelectListItem() { Text = "15", Value = "15" });
            lista_UF.Add(new SelectListItem() { Text = "30", Value = "30" });
            lista_UF.Add(new SelectListItem() { Text = "45", Value = "45" });
            return lista_UF;
        }

        // GET: IntervencaoViewModels
        public ActionResult Index([Form] QueryOptions queryOptions, string doutor, string paciente, string procedimento, string data)
        {
            var intervencaoViewModels = new List<IntervencaoViewModel>();
            var intervencoes = db.Intervencoes.Include(i => i.Especialista).
                Include(i => i.Paciente).Include(i => i.Procedimento).Include(i => i.Arquivo).ToList();

            if (!string.IsNullOrEmpty(doutor))
            {
                intervencoes = intervencoes.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor.ToUpper())).ToList();
                ViewBag.doutor = doutor;
            }
            if (!string.IsNullOrEmpty(paciente))
            {
                intervencoes = intervencoes.Where(x => x.Paciente.Nome.ToUpper().Contains(paciente.ToUpper())).ToList();
                ViewBag.paciente = paciente;
            }
            if (!string.IsNullOrEmpty(procedimento))
            {
                intervencoes = intervencoes.Where(x => x.Procedimento.Nome.ToUpper().Contains(procedimento.ToUpper())).ToList();
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

            if (queryOptions.SortField == "Id")
                queryOptions.SortField = "Date";

            intervencoes = intervencoes.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();
            intervencoes.ForEach(x => intervencaoViewModels.Add(GetIntervencaoViewModel(x)));

            return View(intervencaoViewModels);
        }

        // GET: IntervencaoViewModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intervencao intervencao = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Especialista)
                .Include(x => x.Paciente).Include(x => x.Procedimento).Include(x => x.Arquivo).FirstOrDefault();
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View();
        }

        // POST: IntervencaoViewModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Hora,Minuto,ProcedimentoId,EspecialistaId,"+
            "PacienteId,Observacao,ValorPago,FormaPagamentoId")]IntervencaoViewModel intervencaoViewModel,
            [Bind(Include = "Cpf,Nome,DataNascimento")]Paciente formPaciente,
            string cidade, string bairro, string rua, string numero, string telefone, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    string path = "";

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

                        Arquivo arquivo = new Arquivo();
                        arquivo.OriginalName = "";
                        arquivo.Path = "#";
                        db.Arquivos.Add(arquivo);
                        db.SaveChanges();

                        var intervencao = new Intervencao();
                        intervencao.Date = intervencaoViewModel.Date;
                        intervencao.Hora = intervencaoViewModel.Hora;
                        intervencao.Minuto = intervencaoViewModel.Minuto;
                        intervencao.ProcedimentoId = intervencaoViewModel.ProcedimentoId;
                        intervencao.EspecialistaId = intervencaoViewModel.EspecialistaId;
                        intervencao.PacienteId = paciente.Id;
                        intervencao.Observacao = intervencaoViewModel.Observacao;
                        intervencao.ValorPago = intervencaoViewModel.ValorPago;
                        intervencao.ArquivoId = arquivo.Id;
                        db.Intervencoes.Add(intervencao);
                        db.SaveChanges();

                        if (file != null)
                        {
                            path = Path.Combine(@Server.MapPath(@"~\Files\Clientes"), string.Format("i_{0}_{1}", intervencao.Id, file.FileName));
                            file.SaveAs(path);
                            arquivo.Path = path;
                            arquivo.OriginalName = file.FileName;
                        }


                        var atendimentoArquivo = new AtendimentoArquivo();
                        atendimentoArquivo.ArquivoId = arquivo.Id;
                        atendimentoArquivo.Tipo = "Procedimento";
                        atendimentoArquivo.AtendimentoId = intervencao.Id;
                        db.AtendimentoArquivo.Add(atendimentoArquivo);
                        

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
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        DebugLog.Logar(DebugLog.Details(e));

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        transaction.Rollback();
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
            Intervencao intervencao = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Especialista)
                .Include(x => x.Paciente).Include(x => x.Procedimento).Include(x => x.Arquivo).FirstOrDefault();
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View(intervencaoViewModel);
        }

        // POST: IntervencaoViewModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Hora,Minuto,ProcedimentoId,EspecialistaId,"+
            "PacienteId,Observacao,ValorPago,FormaPagamentoId")] IntervencaoViewModel intervencaoViewModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    string path = "";

                    try
                    {
                        var intervencao = db.Intervencoes.Where(x => x.Id == intervencaoViewModel.Id).FirstOrDefault();

                        if (intervencao != null)
                        {
                            intervencao.Date = intervencaoViewModel.Date;
                            intervencao.Hora = intervencaoViewModel.Hora;
                            intervencao.Minuto = intervencaoViewModel.Minuto;
                            intervencao.ProcedimentoId = intervencaoViewModel.ProcedimentoId;
                            intervencao.EspecialistaId = intervencaoViewModel.EspecialistaId;
                            intervencao.PacienteId = intervencaoViewModel.PacienteId;
                            intervencao.Observacao = intervencaoViewModel.Observacao;
                            intervencao.ValorPago = intervencaoViewModel.ValorPago;
                            db.SaveChanges();

                            Arquivo arquivo = db.Arquivos.Where(x => x.Id == intervencao.ArquivoId).FirstOrDefault();

                            if (arquivo != null)
                            {
                                if (file != null)
                                {

                                    if (System.IO.File.Exists(arquivo.Path))
                                    {
                                        System.IO.File.Delete(arquivo.Path);
                                    }

                                    path = Path.Combine(@Server.MapPath(@"~\Files\Clientes"), string.Format("i_{0}_{1}", intervencao.Id, file.FileName));
                                    file.SaveAs(path);
                                    arquivo.Path = path;
                                    arquivo.OriginalName = file.FileName;
                                }
                            }

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
                        DebugLog.Logar(e.Message);
                        DebugLog.Logar(e.StackTrace);
                        DebugLog.Logar(DebugLog.Details(e));                                            

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                        transaction.Rollback();

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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
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
                 .Include(x => x.Paciente).Include(x => x.Procedimento).Include(x => x.Arquivo).FirstOrDefault();
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
                        int fileId = intervencao.ArquivoId;

                        db.PagamentosProcedimentos.Where(x => x.IntervencaoId == id).ToList().
                            ForEach(x => db.PagamentosProcedimentos.Remove(x));
                        db.SaveChanges();
                        db.Intervencoes.Remove(intervencao);
                        db.SaveChanges();

                        var file = db.Arquivos.Where(x => x.Id == fileId).FirstOrDefault();

                        if (file != null)
                        {
                            if (System.IO.File.Exists(file.Path))
                            {
                                System.IO.File.Delete(file.Path);
                            }
                            db.Arquivos.Remove(file);
                        }


                        var fileAtendimento = db.AtendimentoArquivo.Where(x => x.ArquivoId == fileId).Where(x => x.Tipo == "Procedimento").FirstOrDefault();

                        if (fileAtendimento != null)
                        {
                            db.AtendimentoArquivo.Remove(fileAtendimento);
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

        public ActionResult Export([Form] QueryOptions queryOptions, string doutor, string paciente, string procedimento, string data)
        {
            try
            {
                var intervencaoViewModels = new List<IntervencaoViewModel>();
                var intervencoes = db.Intervencoes.Include(i => i.Especialista).
                    Include(i => i.Paciente).Include(i => i.Procedimento)
                    .Include(i => i.Arquivo).ToList();

                if (!string.IsNullOrEmpty(doutor))
                {
                    intervencoes = intervencoes.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor)).ToList();
                    ViewBag.doutor = doutor;
                }
                if (!string.IsNullOrEmpty(paciente))
                {
                    intervencoes = intervencoes.Where(x => x.Paciente.Nome.ToUpper().Contains(paciente)).ToList();
                    ViewBag.paciente = paciente;
                }
                if (!string.IsNullOrEmpty(procedimento))
                {
                    intervencoes = intervencoes.Where(x => x.Procedimento.Nome.ToUpper().Contains(procedimento)).ToList();
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

                foreach (DataRow row in dt.Rows)
                {
                    var procedimentoId = Int32.Parse(row["Procedimento"].ToString());
                    var especialistaId = Int32.Parse(row["Especialista"].ToString());
                    var pacienteId = Int32.Parse(row["Paciente"].ToString());
                    var formaPagamentoId = Int32.Parse(row["Forma Pagamento"].ToString());

                    var lvprocedimento = db.Procedimentos.Where(x => x.Id == procedimentoId).FirstOrDefault();
                    var lvespecialista = db.Especialistas.Where(x => x.Id == especialistaId).FirstOrDefault();
                    var lvpaciente = db.Pacientes.Where(x => x.Id == pacienteId).FirstOrDefault();
                    var lvformapagamento = db.FormasPagamento.Where(x => x.Id == formaPagamentoId).FirstOrDefault();

                    row["Procedimento"] = lvprocedimento != null ? lvprocedimento.Nome : "";
                    row["Especialista"] = lvespecialista != null ? lvespecialista.Nome : "";
                    row["Paciente"] = lvpaciente != null ? lvpaciente.Nome : "";
                    row["Forma Pagamento"] = lvformapagamento != null ? lvformapagamento.Nome : "";
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

        public ActionResult GetFile(string fileName)
        {
            byte[] fileBytes = null;

            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    fileBytes = System.IO.File.ReadAllBytes(fileName);
                    string lvfileName = fileName.Split(new string[] { @"Clientes\" }, StringSplitOptions.None)[1];
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, lvfileName);
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.Flush();
                Response.End();
                return null;
            }
            catch (Exception)
            {
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.Flush();
                Response.End();
                return null;
            }
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
