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
    public class PacienteViewModelController : Controller
    {
        private HealthContext db = new HealthContext();

        // GET: PacienteViewModel
        public ActionResult Index()
        {
            var pacientesViewModel = new List<PacienteViewModel>();
            var pacientes = db.Pacientes.ToList();

            pacientes.ForEach(x =>
            {
                PacienteViewModel pacienteViewModel = GetPacienteViewModel(x);
                pacientesViewModel.Add(pacienteViewModel);
            });

            return View(pacientesViewModel);
        }

        private PacienteViewModel GetPacienteViewModel(Paciente x)
        {
            var myPhone = db.Telefones.Where(y => y.Id == x.TelefoneId).FirstOrDefault();
            var myAddress = db.Enderecos.Where(y => y.Id == x.EnderecoId).FirstOrDefault();

            var pacienteViewModel = new PacienteViewModel();
            pacienteViewModel.Id = x.Id;
            pacienteViewModel.Nome = x.Nome;
            pacienteViewModel.Cpf = x.Cpf;
            pacienteViewModel.DataNascimento = x.DataNascimento;
            pacienteViewModel.DataCadastro = x.DataCadastro;
            pacienteViewModel.ComoConheceu = x.ComoConheceu;

            if (myPhone != null)
            {
                pacienteViewModel.NumeroTelefone = myPhone.Numero;
            }

            if (myAddress != null)
            {
                pacienteViewModel.Cidade = myAddress.Cidade;
                pacienteViewModel.Bairro = myAddress.Bairro;
                pacienteViewModel.Rua = myAddress.Rua;
                pacienteViewModel.NumeroResidencia = myAddress.Numero;
            }

            return pacienteViewModel;
        }

        // GET: PacienteViewModel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PacienteViewModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Cpf,DataNascimento,DataCadastro,ComoConheceu,Cidade,Bairro,Rua,NumeroResidencia,NumeroTelefone")] PacienteViewModel pacienteViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var endereco = new Endereco();
                        endereco.Cidade = pacienteViewModel.Cidade;
                        endereco.Bairro = pacienteViewModel.Bairro;
                        endereco.Rua = pacienteViewModel.Rua;
                        endereco.Numero = pacienteViewModel.NumeroResidencia;
                        db.Enderecos.Add(endereco);
                        db.SaveChanges();


                        var telefone = new Telefone();
                        telefone.Numero = pacienteViewModel.NumeroTelefone;
                        db.Telefones.Add(telefone);

                        var paciente = new Paciente();
                        paciente.Nome = pacienteViewModel.Nome;
                        paciente.Cpf = pacienteViewModel.Cpf;
                        paciente.DataNascimento = pacienteViewModel.DataNascimento;
                        paciente.DataCadastro = pacienteViewModel.DataCadastro;
                        paciente.ComoConheceu = pacienteViewModel.ComoConheceu;
                        paciente.EnderecoId = endereco.Id;
                        paciente.TelefoneId = telefone.Id;
                        db.Pacientes.Add(paciente);
                        

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

            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // POST: PacienteViewModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Cpf,DataNascimento,DataCadastro,ComoConheceu,Cidade,Bairro,Rua,NumeroResidencia,NumeroTelefone")] PacienteViewModel pacienteViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var paciente = db.Pacientes.Where(x => x.Id == pacienteViewModel.Id).FirstOrDefault();

                        if (paciente != null)
                        {
                            paciente.Nome = pacienteViewModel.Nome;
                            paciente.Cpf = pacienteViewModel.Cpf;
                            paciente.DataNascimento = pacienteViewModel.DataNascimento;
                            paciente.DataCadastro = pacienteViewModel.DataCadastro;
                            paciente.ComoConheceu = pacienteViewModel.ComoConheceu;

                            var endereco = db.Enderecos.Where(x => x.Id == paciente.EnderecoId).FirstOrDefault();

                            if (endereco != null)
                            {
                                endereco.Cidade = pacienteViewModel.Cidade;
                                endereco.Bairro = pacienteViewModel.Bairro;
                                endereco.Rua = pacienteViewModel.Rua;
                                endereco.Numero = pacienteViewModel.NumeroResidencia;
                            }

                            var telefone = db.Telefones.Where(x => x.Id == paciente.TelefoneId).FirstOrDefault();

                            if (telefone != null)
                            {
                                telefone.Numero = pacienteViewModel.NumeroTelefone;
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
            return View(pacienteViewModel);
        }

        // GET: PacienteViewModel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteViewModel pacienteViewModel = GetPacienteViewModel(db.Pacientes.Find(id));
            if (pacienteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(pacienteViewModel);
        }

        // POST: PacienteViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var paciente = db.Pacientes.Where(x => x.Id == id).FirstOrDefault();

                    if (paciente != null)
                    {                       
                        var endereco = db.Enderecos.Where(x => x.Id == paciente.EnderecoId).FirstOrDefault();

                        var telefone = db.Telefones.Where(x => x.Id == paciente.TelefoneId).FirstOrDefault();

                        db.Pacientes.Remove(paciente);
                        
                        if (telefone != null)
                        {
                            db.Telefones.Remove(telefone);
                        }

                        if (endereco != null)
                        {
                            db.Enderecos.Remove(endereco);
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
