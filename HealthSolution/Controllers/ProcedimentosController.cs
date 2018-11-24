using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Dal;
using HealthSolution.Models;
using HealthSolution.Filters;
using HealthSolution.Extensions;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class ProcedimentosController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: Procedimentos
        public ActionResult Index()
        {
            return View(db.Procedimentos.ToList());
        }

        // GET: Procedimentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedimento procedimento = db.Procedimentos.Find(id);
            if (procedimento == null)
            {
                return HttpNotFound();
            }
            return View(procedimento);
        }

        // GET: Procedimentos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Procedimentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Valor")] Procedimento procedimento)
        {
            if (ModelState.IsValid)
            {
                db.Procedimentos.Add(procedimento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(procedimento);
        }

        // GET: Procedimentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedimento procedimento = db.Procedimentos.Find(id);
            if (procedimento == null)
            {
                return HttpNotFound();
            }
            return View(procedimento);
        }

        // POST: Procedimentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Valor")] Procedimento procedimento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(procedimento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(procedimento);
        }

        // GET: Procedimentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procedimento procedimento = db.Procedimentos.Find(id);
            if (procedimento == null)
            {
                return HttpNotFound();
            }
            return View(procedimento);
        }

        // POST: Procedimentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Procedimento procedimento = db.Procedimentos.Find(id);

            if (procedimento != null)
            {
                db.Intervencoes.Where(x => x.ProcedimentoId == id).ToList().
                    ForEach(x => db.Intervencoes.Remove(x));
                db.SaveChanges();
            }
            db.Procedimentos.Remove(procedimento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetIntervencaoHoje()
        {
            DateTime data = DateTime.Now.Date;
            List<Intervencao> Intervencao = new List<Intervencao>();
            Intervencao = db.Intervencoes.Where(x => x.Date == data).OrderBy(x => x.Hora).Include(x => x.Especialista).Include(x => x.Paciente).Include(x => x.Procedimento).ToList();
            return Json(Intervencao);            
        }

        public ActionResult GetProcedimentos(string pesquisar)
        {
            DateTime data = DateTime.Now.Date;
            List<Intervencao> intervencao = new List<Intervencao>();
            intervencao = db.Intervencoes.Where(x => x.Paciente.Nome.Contains(pesquisar) || x.Procedimento.Nome.Contains(pesquisar) || x.Especialista.Nome.Contains(pesquisar)).Where(x => x.Date == data).Include(x => x.Especialista).Include(x => x.Paciente).Include(x => x.Procedimento).OrderBy(x => x.Date).ToList();
            return Json(intervencao);
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
