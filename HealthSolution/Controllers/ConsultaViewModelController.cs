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
            consultaViewModel.LinkArquivo = x.Arquivo.Path;

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

        // GET: ConsultaViewModel
        public ActionResult Index([Form] QueryOptions queryOptions, string doutor, string paciente, string especialidade, string data)
        {
            var consultasViewModels = new List<ConsultaViewModel>();
            var consultas = db.Consultas.Include(x => x.Especialista).Include(x => x.Especialidade)
                .Include(x => x.Paciente).Include(x => x.Arquivo).ToList();

            if (!string.IsNullOrEmpty(doutor))
            {
                consultas = consultas.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor.ToUpper())).ToList();
                ViewBag.doutor = doutor;
            }
            if (!string.IsNullOrEmpty(paciente))
            {
                consultas = consultas.Where(x => x.Paciente.Nome.ToUpper().Contains(paciente.ToUpper())).ToList();
                ViewBag.paciente = paciente;
            }
            if (!string.IsNullOrEmpty(especialidade))
            {
                consultas = consultas.Where(x => x.Especialidade.Nome.ToUpper().Contains(especialidade.ToUpper())).ToList();
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

            if (queryOptions.SortField == "Id")
                queryOptions.SortField = "Date";
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
                Include(x => x.Paciente).Include(x => x.Arquivo).FirstOrDefault();
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
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
                        
                        var consulta = new Consulta();
                        consulta.Date = consultaViewModel.Date;
                        consulta.Hora = consultaViewModel.Hora;
                        consulta.Minuto = consultaViewModel.Minuto;
                        consulta.EspecialidadeId = consultaViewModel.EspecialidadeId;
                        consulta.EspecialistaId = consultaViewModel.EspecialistaId;
                        consulta.PacienteId = paciente.Id;
                        consulta.Observacao = consultaViewModel.Observacao;
                        consulta.ValorPago = consultaViewModel.ValorPago;
                        consulta.ArquivoId = arquivo.Id;
                        consulta.AtendimentoRealizado = false;
                        db.Consultas.Add(consulta);
                        db.SaveChanges();


                        var atendimentoArquivo = new AtendimentoArquivo();
                        atendimentoArquivo.ArquivoId = arquivo.Id;
                        atendimentoArquivo.Tipo = "Consulta";
                        atendimentoArquivo.AtendimentoId = consulta.Id;
                        db.AtendimentoArquivo.Add(atendimentoArquivo);
                        db.SaveChanges();

                        if (file != null)
                        {
                            path = Path.Combine(@Server.MapPath(@"~\Files\Clientes"), string.Format("c_{0}_{1}", consulta.Id, file.FileName));
                            file.SaveAs(path);
                            arquivo.Path = path;
                            arquivo.OriginalName = file.FileName;
                        }

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

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome", consultaViewModel.EspecialidadeId);
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", consultaViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", consultaViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", consultaViewModel.PacienteId);
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View(consultaViewModel);
        }

        // GET: ConsultaViewModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var consulta = db.Consultas.Where(x => x.Id == id).Include(x => x.Especialidade)
                .Include(x => x.Especialista).Include(x => x.Paciente).Include(x => x.Arquivo).FirstOrDefault();
            if (consulta == null)
            {
                return HttpNotFound();
            }
            ConsultaViewModel consultaViewModel = GetConsultaViewModel(consulta);
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View(consultaViewModel);
        }

        // POST: ConsultaViewModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Hora,Minuto,EspecialidadeId,EspecialistaId,PacienteId,"+
            "Observacao,ValorPago,FormaPagamentoId")] ConsultaViewModel consultaViewModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    string path = "";

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

                            Arquivo arquivo = db.Arquivos.Where(x => x.Id == consulta.ArquivoId).FirstOrDefault();

                            if(arquivo != null)
                            {
                                if (file != null)
                                {

                                    if (System.IO.File.Exists(arquivo.Path))
                                    {
                                        System.IO.File.Delete(arquivo.Path);
                                    }

                                    path = Path.Combine(@Server.MapPath(@"~\Files\Clientes"), string.Format("c_{0}_{1}", consulta.Id, file.FileName));
                                    file.SaveAs(path);
                                    arquivo.Path = path;
                                    arquivo.OriginalName = file.FileName;
                                }
                            }                     

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

            ViewBag.EspecialidadeId = new SelectList(db.Especialidades, "Id", "Nome", consultaViewModel.EspecialidadeId);
            ViewBag.EspecialistaId = new SelectList(db.Especialistas, "Id", "Nome", consultaViewModel.EspecialistaId);
            ViewBag.FormaPagamentoId = new SelectList(paymentWays, "Id", "Nome", consultaViewModel.FormaPagamentoId);
            ViewBag.PacienteId = new SelectList(db.Pacientes, "Id", "Nome", consultaViewModel.PacienteId);
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View(consultaViewModel);
        }

        public ActionResult GetConsultasHoje()
        {
            DateTime data = DateTime.Now.Date;
            List<Consulta> consultas = new List<Consulta>();
            consultas = db.Consultas.Where(x => x.Date == data).Include(x => x.Paciente).Include(x => x.Especialista).Include(x => x.Especialidade).ToList();
            return Json(consultas);
        }

        public ActionResult GetConsulta(string pesquisar)
        {
            DateTime data = DateTime.Now.Date;
            List<Consulta> consultas = new List<Consulta>();
            consultas = db.Consultas.Where(x => x.Paciente.Nome.Contains(pesquisar) || x.Especialidade.Nome.Contains(pesquisar) || x.Especialista.Nome.Contains(pesquisar)).Where(x => x.Date == data).Include(x => x.Especialista).Include(x => x.Paciente).Include(x => x.Especialidade).OrderBy(x => x.Date).ToList();
            return Json(consultas);
        }

        // GET: ConsultaViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var consulta = db.Consultas.Where(x => x.Id == id).Include(x => x.Especialidade).
                            Include(x => x.Especialista).Include(x => x.Paciente).Include(x => x.Arquivo).FirstOrDefault();
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
                        int fileId = consulta.ArquivoId;

                        db.PagamentosConsultas.Where(x => x.ConsultaId == id).ToList().
                            ForEach(x => db.PagamentosConsultas.Remove(x));
                        db.SaveChanges();
                        db.Consultas.Remove(consulta);
                        db.SaveChanges();

                        var file = db.Arquivos.Where(x => x.Id == fileId).FirstOrDefault();

                        if(file != null)
                        {
                            if (System.IO.File.Exists(file.Path))
                            {
                                System.IO.File.Delete(file.Path);
                            }
                            db.Arquivos.Remove(file);
                            
                        }

                        var fileAtendimento = db.AtendimentoArquivo.Where(x => x.ArquivoId == fileId).Where(x => x.Tipo == "Consulta").FirstOrDefault();

                        if(fileAtendimento != null)
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

        public ActionResult Export([Form] QueryOptions queryOptions, string doutor, string paciente, string especialidade, string data)
        {
            try
            {
                var consultasViewModels = new List<ConsultaViewModel>();
                var consultas = db.Consultas.Include(x => x.Especialista).Include(x => x.Especialidade)
                    .Include(x => x.Paciente).Include(x => x.Arquivo).ToList();

                if (!string.IsNullOrEmpty(doutor))
                {
                    consultas = consultas.Where(x => x.Especialista.Nome.ToUpper().Contains(doutor)).ToList();
                    ViewBag.doutor = doutor;
                }
                if (!string.IsNullOrEmpty(paciente))
                {
                    consultas = consultas.Where(x => x.Paciente.Nome.ToUpper().Contains(paciente)).ToList();
                    ViewBag.paciente = paciente;
                }
                if (!string.IsNullOrEmpty(especialidade))
                {
                    consultas = consultas.Where(x => x.Especialidade.Nome.ToUpper().Contains(especialidade)).ToList();
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

                foreach (DataRow row in dt.Rows)
                {
                    var especialidadeId = Int32.Parse(row["Especialidade"].ToString());
                    var especialistaId = Int32.Parse(row["Especialista"].ToString());
                    var pacienteId = Int32.Parse(row["Paciente"].ToString());
                    var formaPagamentoId = Int32.Parse(row["Forma Pagamento"].ToString());

                    var lvespecialidade = db.Especialidades.Where(x => x.Id == especialidadeId).FirstOrDefault();
                    var lvespecialista = db.Especialistas.Where(x => x.Id == especialistaId).FirstOrDefault();
                    var lvpaciente = db.Pacientes.Where(x => x.Id == pacienteId).FirstOrDefault();
                    var lvformapagamento = db.FormasPagamento.Where(x => x.Id == formaPagamentoId).FirstOrDefault();

                    row["Especialidade"] = lvespecialidade != null ? lvespecialidade.Nome : "";
                    row["Especialista"] = lvespecialista != null ? lvespecialista.Nome : "";
                    row["Paciente"] = lvpaciente != null ? lvpaciente.Nome : "";
                    row["Forma Pagamento"] = lvformapagamento != null ? lvformapagamento.Nome : "";
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
