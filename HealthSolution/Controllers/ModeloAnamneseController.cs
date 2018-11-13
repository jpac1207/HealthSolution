﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Dal;
using HealthSolution.Models;

namespace HealthSolution.Controllers
{
    public class ModeloAnamneseController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: ModeloAnamnese
        public ActionResult Index()
        {
            return View(db.ModelosAnamneses.ToList());
        }

        // GET: ModeloAnamnese/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloAnamnese modeloAnamnese = db.ModelosAnamneses.Find(id);
            if (modeloAnamnese == null)
            {
                return HttpNotFound();
            }
            return View(modeloAnamnese);
        }

        // GET: ModeloAnamnese/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ModeloAnamnese/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Modelo")] ModeloAnamnese modeloAnamnese)
        {
            if (ModelState.IsValid)
            {
                db.ModelosAnamneses.Add(modeloAnamnese);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(modeloAnamnese);
        }

        // GET: ModeloAnamnese/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloAnamnese modeloAnamnese = db.ModelosAnamneses.Find(id);
            if (modeloAnamnese == null)
            {
                return HttpNotFound();
            }
            return View(modeloAnamnese);
        }

        // POST: ModeloAnamnese/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Modelo")] ModeloAnamnese modeloAnamnese)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modeloAnamnese).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modeloAnamnese);
        }

        // GET: ModeloAnamnese/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloAnamnese modeloAnamnese = db.ModelosAnamneses.Find(id);
            if (modeloAnamnese == null)
            {
                return HttpNotFound();
            }
            return View(modeloAnamnese);
        }

        // POST: ModeloAnamnese/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModeloAnamnese modeloAnamnese = db.ModelosAnamneses.Find(id);
            db.ModelosAnamneses.Remove(modeloAnamnese);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult GetModeloAnamneseById(int id)
        {
            var modelo = db.ModelosAnamneses.Where(x => x.Id == id).FirstOrDefault();
            
            return Json(modelo);
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
