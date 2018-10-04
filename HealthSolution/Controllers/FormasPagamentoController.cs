using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Models;
using HealthSolution.Dal;
using HealthSolution.Filters;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class FormasPagamentoController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: FormasPagamento
        public ActionResult Index()
        {
            return View(db.FormasPagamento.ToList());
        }

        // GET: FormasPagamento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamento formaPagamento = db.FormasPagamento.Find(id);
            if (formaPagamento == null)
            {
                return HttpNotFound();
            }
            return View(formaPagamento);
        }

        // GET: FormasPagamento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FormasPagamento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome")] FormaPagamento formaPagamento)
        {
            if (ModelState.IsValid)
            {
                db.FormasPagamento.Add(formaPagamento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formaPagamento);
        }

        // GET: FormasPagamento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamento formaPagamento = db.FormasPagamento.Find(id);
            if (formaPagamento == null)
            {
                return HttpNotFound();
            }
            return View(formaPagamento);
        }

        // POST: FormasPagamento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome")] FormaPagamento formaPagamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formaPagamento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formaPagamento);
        }

        // GET: FormasPagamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormaPagamento formaPagamento = db.FormasPagamento.Find(id);
            if (formaPagamento == null)
            {
                return HttpNotFound();
            }
            return View(formaPagamento);
        }

        // POST: FormasPagamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FormaPagamento formaPagamento = db.FormasPagamento.Find(id);
            db.FormasPagamento.Remove(formaPagamento);
            db.SaveChanges();
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
