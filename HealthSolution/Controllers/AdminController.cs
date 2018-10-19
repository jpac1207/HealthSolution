using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HealthSolution.Extensions;
using HealthSolution.Dal;

namespace HealthSolution.Controllers
{
    public class AdminController : Controller
    {
        HealthContext db = new HealthContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Message = "Os campos de usuário e senha são obrigatórios!";
                    ViewBag.StyleClass = "alert alert-danger";
                    return View("Index");
                }
                else
                {
                    var lvUser = db.Usuarios.Where(x => x.Name == user).FirstOrDefault();

                    if (lvUser == null)
                    {
                        ViewBag.Message = "Usuário não encontrado!";
                        ViewBag.StyleClass = "alert alert-danger";
                        return View("Index");
                    }
                    else
                    {
                        if (HashUtil.VerifyHash(password, lvUser.HashValue))
                        {
                            ViewBag.Message = "Logado com sucesso!";
                            ViewBag.StyleClass = "alert alert-success";
                            Session["userId"] = lvUser.Name;
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Message = "Senha incorreta!";
                            ViewBag.StyleClass = "alert alert-danger";
                            return View("Index");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                DebugLog.Logar(e.StackTrace);
                ViewBag.Message = "Não foi possível conectar ao banco de dados!";
                ViewBag.StyleClass = "alert alert-danger";
            }
            return View("Index");
        }
                
        public ActionResult Logout()
        {
            Session["userId"] = null;
            return RedirectToAction("Index", "Admin");
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