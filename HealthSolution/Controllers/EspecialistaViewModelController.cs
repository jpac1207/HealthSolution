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
           
            if (myespeciality != null)
            {
                var especiality = db.Especialidades.Where(y => y.Id == myespeciality.EspecialidadeId).FirstOrDefault();

                if (especiality != null)
                {
                    especialistaViewModel.especialidade = especiality.Nome;
                    especialistaViewModel.EspecialidadeId = especiality.Id;
                } 
            }
            
            return especialistaViewModel;
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
            return View();
        }

        // POST: EspecialistaViewModel/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Crm,EspecialidadeId,especialidade")] EspecialistaViewModel especialistaViewModel)
        {

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var especialista = new Especialista();
                        especialista.Nome = especialistaViewModel.Nome;
                        especialista.Crm = especialistaViewModel.Crm;
                        db.Especialistas.Add(especialista);
                        db.SaveChanges();

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
            return View(especialistaViewModel);
        }

        // POST: EspecialistaViewModel/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Crm,EspecialidadeId,especialidade")] EspecialistaViewModel especialistaViewModel)
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
            EspecialistaViewModel especialistaViewModel = GetEspecialistaViewModel(db.Especialistas.Find(id));
           // db.Especialistas.Remove(especialistaViewModel);
           // db.SaveChanges();
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
