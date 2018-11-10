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
using HealthSolution.Extensions;
using HealthSolution.Filters;

namespace HealthSolution.Controllers
{
    [AuthenticationFilter]
    public class UsuariosController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: Usuarios
        public ActionResult Index()
        {
            return View(db.Usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HashValue,IsMedical")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var previous = db.Usuarios.Where(x => x.Name.ToUpper() == usuario.Name.ToUpper()).FirstOrDefault();

                if (previous == null)
                {
                    usuario.HashValue = HashUtil.ComputeHash(usuario.HashValue, null);
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Usuário já existe!";
                    ViewBag.StyleClass = "alert alert-danger";
                    return View("Index");
                }
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(new Usuario() { Id = usuario.Id, Name = usuario.Name, HashValue = "" });
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HashValue,IsMedical")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var previous = db.Usuarios.Where(x => x.Id == usuario.Id).FirstOrDefault();

                if (previous != null)
                {
                    previous.Name = usuario.Name;
                    previous.IsMedical = usuario.IsMedical;

                    if (!string.IsNullOrEmpty(usuario.HashValue))
                        previous.HashValue = HashUtil.ComputeHash(usuario.HashValue, null);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
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
