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
    public class EspecialistaViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: EspecialistaViewModel
        public ActionResult Index([Form] QueryOptions queryOptions, string nome, string crm, string especialidade)
        {
            var especialistasViewModel = new List<EspecialistaViewModel>();
            var especialistas = new List<Especialista>();

            if (!string.IsNullOrEmpty(nome))
            {
                especialistas = db.Especialistas.Where(x => x.Nome.ToUpper().Contains(nome.ToUpper())).ToList();
                ViewBag.nome = nome;
            }
            else
            {
                especialistas = db.Especialistas.ToList();
            }

            if (!string.IsNullOrEmpty(crm))
            {
                especialistas = especialistas.Where(x => x.Crm.Equals(crm)).ToList();
                ViewBag.crm = crm;
            }

            if (!string.IsNullOrEmpty(especialidade))
            {
                var lvespecialistas = new List<Especialista>();

                especialistas.ForEach(x =>
                {
                    var lvEspecialidadeEspecialista = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == x.Id)
                    .Include(y => y.Especialidade).FirstOrDefault();

                    if (lvEspecialidadeEspecialista != null &&
                    lvEspecialidadeEspecialista.Especialidade.Nome.ToUpper().Contains(especialidade.ToUpper()))
                        lvespecialistas.Add(x);
                });

                especialistas = lvespecialistas;
                ViewBag.especialidade = especialidade;
            }

            queryOptions.SortOrder = SortOrder.DESC;
            var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
            queryOptions.TotalPages = (int)Math.Ceiling((double)especialistas.Count() / queryOptions.PageSize);
            ViewBag.QueryOptions = queryOptions;

            especialistas = especialistas.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

            especialistas.ForEach(x =>
            {
                EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(x);
                especialistasViewModel.Add(especialistaViewModel);
            });

            return View(especialistasViewModel);
        }

        private EspecialistaViewModel GetEspecialistaViewModel(Especialista x)
        {
            var myespeciality = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == x.Id).FirstOrDefault();

            var especialistaViewModel = new EspecialistaViewModel();
            especialistaViewModel.Id = x.Id;
            especialistaViewModel.Nome = x.Nome;
            especialistaViewModel.Crm = x.Crm;
            especialistaViewModel.Email = x.Email;
            especialistaViewModel.DataNascimento = x.DataNascimento;
            especialistaViewModel.ConselhoUF = x.ConselhoUF;

            if (myespeciality != null)
            {
                var especiality = db.Especialidades.Where(y => y.Id == myespeciality.EspecialidadeId).FirstOrDefault();

                if (especiality != null)
                {
                    especialistaViewModel.Especialidade = especiality.Nome;
                    especialistaViewModel.EspecialidadeId = especiality.Id;
                }
            }

            if (x.EnderecoId != -1)
            {
                var endereco = db.Enderecos.Where(y => y.Id == x.EnderecoId).FirstOrDefault();
                especialistaViewModel.Bairro = endereco.Bairro;
                especialistaViewModel.Cidade = endereco.Cidade;
                especialistaViewModel.Numero = endereco.Numero;
                especialistaViewModel.Rua = endereco.Rua;
            }

            if (x.TelefoneId != -1)
            {
                var telefone = db.Telefones.Where(y => y.Id == x.TelefoneId).FirstOrDefault();
                especialistaViewModel.Telefone = telefone.Numero;
            }

            if (x.DiasAtendimentoId != -1)
            {
                var diasAtendimento = db.DiasAtendimentos.Where(y => y.Id == x.DiasAtendimentoId).FirstOrDefault();
                especialistaViewModel.AtendeSegunda = diasAtendimento.AtendeSegunda;
                especialistaViewModel.AtendeTerca = diasAtendimento.AtendeTerca;
                especialistaViewModel.AtendeQuarta = diasAtendimento.AtendeQuarta;
                especialistaViewModel.AtendeQuinta = diasAtendimento.AtendeQuinta;
                especialistaViewModel.AtendeSexta = diasAtendimento.AtendeSexta;
                especialistaViewModel.AtendeSabado = diasAtendimento.AtendeSabado;
                especialistaViewModel.AtendeDomingo = diasAtendimento.AtendeDomingo;
            }

            return especialistaViewModel;
        }

        private List<SelectListItem> GetListUF()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();

            lista_UF.Add(new SelectListItem() { Text = "AC", Value = "AC" });
            lista_UF.Add(new SelectListItem() { Text = "AL", Value = "AL" });
            lista_UF.Add(new SelectListItem() { Text = "AM", Value = "AM" });
            lista_UF.Add(new SelectListItem() { Text = "AP", Value = "AP" });
            lista_UF.Add(new SelectListItem() { Text = "BA", Value = "BA" });
            lista_UF.Add(new SelectListItem() { Text = "CE", Value = "CE" });
            lista_UF.Add(new SelectListItem() { Text = "DF", Value = "DF" });
            lista_UF.Add(new SelectListItem() { Text = "ES", Value = "ES" });
            lista_UF.Add(new SelectListItem() { Text = "GO", Value = "GO" });
            lista_UF.Add(new SelectListItem() { Text = "MA", Value = "MA" });
            lista_UF.Add(new SelectListItem() { Text = "MG", Value = "MG" });
            lista_UF.Add(new SelectListItem() { Text = "MS", Value = "MS" });
            lista_UF.Add(new SelectListItem() { Text = "MT", Value = "MT" });
            lista_UF.Add(new SelectListItem() { Text = "PA", Value = "PA" });
            lista_UF.Add(new SelectListItem() { Text = "PB", Value = "PB" });
            lista_UF.Add(new SelectListItem() { Text = "PE", Value = "PI" });
            lista_UF.Add(new SelectListItem() { Text = "AC", Value = "AC" });
            lista_UF.Add(new SelectListItem() { Text = "PR", Value = "PR" });
            lista_UF.Add(new SelectListItem() { Text = "RJ", Value = "RJ" });
            lista_UF.Add(new SelectListItem() { Text = "RN", Value = "RN" });
            lista_UF.Add(new SelectListItem() { Text = "RO", Value = "RO" });
            lista_UF.Add(new SelectListItem() { Text = "RS", Value = "RS" });
            lista_UF.Add(new SelectListItem() { Text = "SC", Value = "SC" });
            lista_UF.Add(new SelectListItem() { Text = "SE", Value = "SE" });
            lista_UF.Add(new SelectListItem() { Text = "SP", Value = "SP" });
            lista_UF.Add(new SelectListItem() { Text = "TO", Value = "TO" });

            return lista_UF;
        }

        // GET: EspecialistaViewModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(db.Especialistas.Find(id));
            if (especialistaViewModel == null)
            {
                return HttpNotFound();
            }
            return View(especialistaViewModel);
        }

        // GET: EspecialistaViewModel/Create
        public ActionResult Create()
        {
            ViewBag.Especialidades = db.Especialidades.ToList();
            ViewBag.ListaUF = GetListUF();
            return View();
        }

        // POST: EspecialistaViewModel/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Crm,ConselhoUF,EspecialidadeId,Especialidade,"+
            "DataNascimento,Email,Rua,Bairro,Cidade,Numero,Telefone,"+
            "AtendeSegunda,AtendeTerca,AtendeQuarta,AtendeQuinta,AtendeSexta,AtendeSabado,AtendeDomingo")] EspecialistaViewModel especialistaViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        /* Telefone */
                        var telefone = new Telefone();
                        telefone.Numero = especialistaViewModel.Telefone;
                        db.Telefones.Add(telefone);

                        /* Endereço */
                        var endereco = new Endereco();
                        endereco.Bairro = especialistaViewModel.Bairro;
                        endereco.Cidade = especialistaViewModel.Cidade;
                        endereco.Rua = especialistaViewModel.Rua;
                        endereco.Numero = especialistaViewModel.Numero;
                        db.Enderecos.Add(endereco);
                        db.SaveChanges();

                        /*Dias de Atendimento*/
                        var diasAtendimento = new DiasAtendimento();
                        diasAtendimento.AtendeSegunda = especialistaViewModel.AtendeSegunda;
                        diasAtendimento.AtendeTerca = especialistaViewModel.AtendeTerca;
                        diasAtendimento.AtendeQuarta = especialistaViewModel.AtendeQuarta;
                        diasAtendimento.AtendeQuinta = especialistaViewModel.AtendeQuinta;
                        diasAtendimento.AtendeSexta = especialistaViewModel.AtendeSexta;
                        diasAtendimento.AtendeSabado = especialistaViewModel.AtendeSabado;
                        diasAtendimento.AtendeDomingo = especialistaViewModel.AtendeDomingo;
                        db.DiasAtendimentos.Add(diasAtendimento);
                        db.SaveChanges();

                        /* Especialista */
                        var especialista = new Especialista();
                        especialista.Nome = especialistaViewModel.Nome;
                        especialista.Crm = especialistaViewModel.Crm;
                        especialista.Email = especialistaViewModel.Email;
                        especialista.ConselhoUF = especialistaViewModel.ConselhoUF;
                        especialista.EnderecoId = endereco.Id;
                        especialista.TelefoneId = telefone.Id;
                        especialista.DiasAtendimentoId = diasAtendimento.Id;
                        especialista.DataNascimento = especialistaViewModel.DataNascimento;
                        db.Especialistas.Add(especialista);
                        db.SaveChanges();

                        /* EspecialistaEspecialidade */
                        var especialistaEspecialidade = new EspecialistaEspecialidade();
                        especialistaEspecialidade.EspecialistaId = especialista.Id;
                        especialistaEspecialidade.EspecialidadeId = especialistaViewModel.EspecialidadeId;
                        db.EspecialistasEspecialidades.Add(especialistaEspecialidade);
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

            return View(especialistaViewModel);
        }

        // GET: EspecialistaViewModel/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(db.Especialistas.Find(id));
            if (especialistaViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListaUF = GetListUF();
            ViewBag.Especialidades = db.Especialidades.ToList();

            return View(especialistaViewModel);
        }

        // POST: EspecialistaViewModel/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Crm,ConselhoUF,EspecialidadeId,Especialidade,DataNascimento"+
            ",Email,Rua,Bairro,Cidade,Numero,Telefone,"+
            "AtendeSegunda,AtendeTerca,AtendeQuarta,AtendeQuinta,AtendeSexta,AtendeSabado,AtendeDomingo")] EspecialistaViewModel especialistaViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        /* Especialista */
                        var especialista = db.Especialistas.Where(x => x.Id == especialistaViewModel.Id)
                            .Include(x => x.Telefone).FirstOrDefault();

                        if (especialista != null)
                        {
                            especialista.Nome = especialistaViewModel.Nome;
                            especialista.Crm = especialistaViewModel.Crm;
                            especialista.Email = especialistaViewModel.Email;
                            especialista.ConselhoUF = especialistaViewModel.ConselhoUF;
                            especialista.DataNascimento = especialistaViewModel.DataNascimento;
                            db.SaveChanges();

                            /*Dias de Atendimento*/
                            var diasAtendimento = db.DiasAtendimentos.Where(x => x.Id == especialista.DiasAtendimentoId).FirstOrDefault();

                            if (diasAtendimento != null)
                            {
                                diasAtendimento.AtendeSegunda = especialistaViewModel.AtendeSegunda;
                                diasAtendimento.AtendeTerca = especialistaViewModel.AtendeTerca;
                                diasAtendimento.AtendeQuarta = especialistaViewModel.AtendeQuarta;
                                diasAtendimento.AtendeQuinta = especialistaViewModel.AtendeQuinta;
                                diasAtendimento.AtendeSexta = especialistaViewModel.AtendeSexta;
                                diasAtendimento.AtendeSabado = especialistaViewModel.AtendeSabado;
                                diasAtendimento.AtendeDomingo = especialistaViewModel.AtendeDomingo;
                                db.SaveChanges();
                            }

                            /* Telefone */
                            var telefone = especialista.Telefone;

                            if (telefone != null)
                            {
                                telefone.Numero = especialistaViewModel.Telefone;
                            }

                            /* Endereço */
                            var endereco = db.Enderecos.Where(x => x.Id == especialista.EnderecoId).FirstOrDefault();

                            if (endereco != null)
                            {
                                endereco.Bairro = especialistaViewModel.Bairro;
                                endereco.Cidade = especialistaViewModel.Cidade;
                                endereco.Rua = especialistaViewModel.Rua;
                                endereco.Numero = especialistaViewModel.Numero;
                            }

                            /* EspecialistaEspecialidade */
                            var especialistaEspecialidade = db.EspecialistasEspecialidades.Where(x => x.EspecialistaId == especialista.Id).FirstOrDefault();

                            if (especialistaEspecialidade != null)
                            {
                                especialistaEspecialidade.EspecialistaId = especialista.Id;
                                especialistaEspecialidade.EspecialidadeId = especialistaViewModel.EspecialidadeId;
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
            return View(especialistaViewModel);
        }

        // GET: EspecialistaViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var especialista = db.Especialistas.Where(x => x.Id == id).Include(x => x.Endereco).Include(x => x.Telefone).FirstOrDefault();
            if (especialista == null)
            {
                return HttpNotFound();
            }

            EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(especialista);
            if (especialistaViewModel == null)
            {
                return HttpNotFound();
            }
            return View(especialistaViewModel);
        }

        // POST: EspecialistaViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var especialista = db.Especialistas.Where(x => x.Id == id).Include(x => x.Endereco)
                        .Include(x => x.Telefone).Include(x => x.DiasAtendimento).FirstOrDefault();

                    if (especialista != null)
                    {
                        var lvendereco = especialista.Endereco;
                        var lvtelefone = especialista.Telefone;
                        var lvdiasAtendimento = especialista.DiasAtendimento;

                        var especialistaespecialidade = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == especialista.Id).ToList();
                        especialistaespecialidade.ForEach(x => db.EspecialistasEspecialidades.Remove(x));
                        db.Especialistas.Remove(especialista);
                        db.SaveChanges();

                        if (lvendereco != null)
                        {
                            db.Enderecos.Remove(lvendereco);
                        }

                        if (lvtelefone != null)
                        {
                            db.Telefones.Remove(lvtelefone);
                        }

                        if (lvdiasAtendimento != null)
                        {
                            db.DiasAtendimentos.Remove(lvdiasAtendimento);
                        }
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    DebugLog.Logar(e.Message);
                    DebugLog.Logar(e.StackTrace);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetEspecialistasById(int especialidadeId)
        {
            List<EspecialistaEspecialidade> especialistaEspecialidade = db.EspecialistasEspecialidades.Where(x => x.EspecialidadeId == especialidadeId).Include(x => x.Especialista).ToList();
            return Json(especialistaEspecialidade);
        }

        public ActionResult Export([Form] QueryOptions queryOptions, string nome, string crm, string especialidade)
        {
            try
            {
                var especialistasViewModel = new List<EspecialistaViewModel>();
                var especialistas = new List<Especialista>();

                if (!string.IsNullOrEmpty(nome))
                {
                    especialistas = db.Especialistas.Where(x => x.Nome.Contains(nome)).ToList();
                    ViewBag.nome = nome;
                }
                else
                {
                    especialistas = db.Especialistas.ToList();
                }

                if (!string.IsNullOrEmpty(crm))
                {
                    especialistas = especialistas.Where(x => x.Crm.Equals(crm)).ToList();
                    ViewBag.crm = crm;
                }

                if (!string.IsNullOrEmpty(especialidade))
                {
                    var lvespecialistas = new List<Especialista>();

                    especialistas.ForEach(x =>
                    {
                        var lvEspecialidade = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == x.Id)
                        .Include(y => y.Especialidade).FirstOrDefault();

                        if (lvEspecialidade != null && lvEspecialidade.Especialidade.Nome.Contains(especialidade))
                            lvespecialistas.Add(x);
                    });

                    especialistas = lvespecialistas;
                    ViewBag.especialidade = especialidade;
                }

                queryOptions.SortOrder = SortOrder.DESC;
                var start = (queryOptions.CurrentPage - 1) * queryOptions.PageSize;
                queryOptions.TotalPages = (int)Math.Ceiling((double)especialistas.Count() / queryOptions.PageSize);
                ViewBag.QueryOptions = queryOptions;

                especialistas = especialistas.OrderBy(queryOptions.Sort).Skip(start).Take(queryOptions.PageSize).ToList();

                especialistas.ForEach(x =>
                {
                    EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(x);
                    especialistasViewModel.Add(especialistaViewModel);
                });
                DataTable dt = Utility.ExportListToDataTable(especialistas);

                int enderecoCell = 6;
                int telefoneCell = 7;
                int diasAtendimentoCell = 8;

                foreach (DataRow row in dt.Rows)
                {
                    var enderecoId = Int32.Parse(row[enderecoCell].ToString());
                    var telefoneId = Int32.Parse(row[telefoneCell].ToString());
                    var diasAtendimentoId = Int32.Parse(row[diasAtendimentoCell].ToString());

                    var lvendereco = db.Enderecos.Where(x => x.Id == enderecoId).FirstOrDefault();
                    var lvtelefone = db.Telefones.Where(x => x.Id == telefoneId).FirstOrDefault();
                    var lvdiasAtendimento = db.DiasAtendimentos.Where(x => x.Id == diasAtendimentoId).FirstOrDefault();

                    row[enderecoCell] = lvendereco != null ? (lvendereco.Cidade + " " + lvendereco.Bairro + " " +
                        lvendereco.Rua + " " + lvendereco.Numero) : "";
                    row[telefoneCell] = lvtelefone != null ? lvtelefone.Numero : "";
                    row[diasAtendimentoCell] = lvdiasAtendimento != null ?
                    ((lvdiasAtendimento.AtendeSegunda ? "|Seg|" : "") +
                    (lvdiasAtendimento.AtendeTerca ? "|Ter|" : "") +
                    (lvdiasAtendimento.AtendeQuarta ? "|Qua|" : "") +
                    (lvdiasAtendimento.AtendeQuinta ? "|Quin|" : "") +
                    (lvdiasAtendimento.AtendeSexta ? "|Sex|" : "") +
                    (lvdiasAtendimento.AtendeSabado ? "|Sab|" : "") +
                    (lvdiasAtendimento.AtendeDomingo ? "|Dom|" : "")) : "";
                }

                var gridView = new GridView();
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                string fileName = "Export_Especialistas_" + DateTime.Now.ToString("dd.MM.yyyy") + ".xls";

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

            return Index(queryOptions, nome, crm, especialidade);
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
