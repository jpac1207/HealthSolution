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
    public class EspecialistaViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: EspecialistaViewModel
        public ActionResult Index(string nome, string crm)
        {
            var especialistasViewModel = new List<EspecialistaViewModel>();
            var especialistas = new List<Especialista>();

            if (!string.IsNullOrEmpty(nome))
            {
                especialistas = db.Especialistas.Where(x => x.Nome.Contains(nome)).ToList();
            }
            else
            {
                especialistas = db.Especialistas.ToList();
            }

            if (!string.IsNullOrEmpty(crm))
            {
                especialistas = especialistas.Where(x => x.Crm.Equals(crm)).ToList();
            }

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

            return especialistaViewModel;
        }

        private List<SelectListItem> GetListUF()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();

            lista_UF.Add(new SelectListItem() { Text = "AC" , Value = "AC" });           
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
        public ActionResult Create([Bind(Include = "Id,Nome,Crm,ConselhoUF,EspecialidadeId,Especialidade,DataNascimento,Email,Rua,Bairro,Cidade,Numero,Telefone")] EspecialistaViewModel especialistaViewModel)
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

                        /* Especialista */
                        var especialista = new Especialista();
                        especialista.Nome = especialistaViewModel.Nome;
                        especialista.Crm = especialistaViewModel.Crm;
                        especialista.Email = especialistaViewModel.Email;
                        especialista.ConselhoUF = especialistaViewModel.ConselhoUF;
                        especialista.EnderecoId = endereco.Id;
                        especialista.TelefoneId = telefone.Id;
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
        public ActionResult Edit([Bind(Include = "Id,Nome,Crm,ConselhoUF,EspecialidadeId,Especialidade,DataNascimento,Email,Rua,Bairro,Cidade,Numero,Telefone")] EspecialistaViewModel especialistaViewModel)
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

                            /* Telefone */
                            var telefone = especialista.Telefone;

                            if(telefone != null)
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
                    var especialista = db.Especialistas.Where(x => x.Id == id).FirstOrDefault();

                    if (especialista != null)
                    {
                        var especialistaespecialidade = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == especialista.Id).ToList();
                        especialistaespecialidade.ForEach(x => db.EspecialistasEspecialidades.Remove(x));
                        db.Especialistas.Remove(especialista);
                        db.SaveChanges();
                    }

                    if (especialista.Endereco!= null)
                    {
                        var endereco = especialista.Endereco;
                        db.Enderecos.Remove(endereco);
                    }

                    if (especialista.Telefone != null)
                    {
                        var telefone = especialista.Telefone;
                        db.Telefones.Remove(telefone);
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

        [HttpPost]
        public ActionResult GetEspecialistasById(int especialidadeId)
        {
            List<EspecialistaEspecialidade> especialistaEspecialidade = db.EspecialistasEspecialidades.Where(x => x.EspecialidadeId == especialidadeId).Include(x => x.Especialista).ToList();
            return Json(especialistaEspecialidade);
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
