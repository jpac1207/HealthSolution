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
        public ActionResult Index()
        {
            var especialistasViewModel = new List<EspecialistaViewModel>();
            var especialista = db.Especialistas.ToList();

            especialista.ForEach(x =>
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
           
            if (myespeciality != null)
            {
                var especiality = db.Especialidades.Where(y => y.Id == myespeciality.EspecialidadeId).FirstOrDefault();

                if (especiality != null)
                {
                    especialistaViewModel.Especialidade = especiality.Nome;
                    especialistaViewModel.EspecialidadeId = especiality.Id;
                } 
            }

            if(x.EnderecoId != -1)
            {
                var endereco = db.Enderecos.Where(y => y.Id == x.EnderecoId).FirstOrDefault();
                especialistaViewModel.Bairro = endereco.Bairro;
                especialistaViewModel.Cidade = endereco.Cidade;
                especialistaViewModel.Numero = endereco.Numero;
                especialistaViewModel.Rua = endereco.Rua;
            }
            
            return especialistaViewModel;
        }

        private List<String> GetListUF()
        {
            List<String> lista_UF = new List<string>();

            lista_UF.Add("AC");
            lista_UF.Add("AL");
            lista_UF.Add("AM");
            lista_UF.Add("AP");
            lista_UF.Add("BA");
            lista_UF.Add("CE");
            lista_UF.Add("DF");
            lista_UF.Add("ES");
            lista_UF.Add("GO");
            lista_UF.Add("MA");
            lista_UF.Add("MG");
            lista_UF.Add("MS");
            lista_UF.Add("MT");
            lista_UF.Add("PA");
            lista_UF.Add("PB");
            lista_UF.Add("PE");
            lista_UF.Add("PI");
            lista_UF.Add("PR");
            lista_UF.Add("RJ");
            lista_UF.Add("RN");
            lista_UF.Add("RO");
            lista_UF.Add("RS");
            lista_UF.Add("SC");
            lista_UF.Add("SE");
            lista_UF.Add("SP");
            lista_UF.Add("TO");

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
        public ActionResult Create([Bind(Include = "Id,Nome,Crm,EspecialidadeId,Especialidade, DataNascimento,Email, Rua, Bairro, Cidade, Numero ")] EspecialistaViewModel especialistaViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
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

            ViewBag.Especialidades = db.Especialidades.ToList();

            return View(especialistaViewModel);
        }

        // POST: EspecialistaViewModel/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Crm,EspecialidadeId,Especialidade")] EspecialistaViewModel especialistaViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(especialistaViewModel).State = EntityState.Modified;
                db.SaveChanges();
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
            EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(db.Especialistas.Find(id));
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

                    if (especialista.EnderecoId != -1)
                    {
                        var endereco = db.Enderecos.Where(y => y.Id == especialista.EnderecoId).FirstOrDefault();
                        db.Enderecos.Remove(endereco);
                    }

                    if (especialista != null)
                    {
                        var especialistaespecialidade = db.EspecialistasEspecialidades.Where(y => y.EspecialistaId == especialista.Id).ToList();

                        especialistaespecialidade.ForEach(x => db.EspecialistasEspecialidades.Remove(x));

                        db.Especialistas.Remove(especialista);

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
