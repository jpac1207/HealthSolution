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
            especialistaViewModel.HoraInicial = x.HoraInicial;
            especialistaViewModel.MinutoInicial = x.MinutoInicial;
            especialistaViewModel.HoraFinal = x.HoraFinal;
            especialistaViewModel.MinutoFinal = x.MinutoFinal;

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

        private List<SelectListItem> GetListHour()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();

            for (int i = 0; i < 24; i++)
            {
                var text = i < 10 ? "0" + i : i.ToString();
                var item = new SelectListItem() { Text = text, Value = i.ToString() };
                lista_UF.Add(item);
            }

            return lista_UF;
        }

        private List<SelectListItem> GetListMinute()
        {
            List<SelectListItem> lista_UF = new List<SelectListItem>();
            lista_UF.Add(new SelectListItem() { Text = "00", Value = "0" });
            lista_UF.Add(new SelectListItem() { Text = "15", Value = "15" });
            lista_UF.Add(new SelectListItem() { Text = "30", Value = "30" });
            lista_UF.Add(new SelectListItem() { Text = "45", Value = "45" });
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
            return View();
        }

        // POST: EspecialistaViewModel/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Crm,ConselhoUF,EspecialidadeId,Especialidade,"+
            "DataNascimento,Email,Rua,Bairro,Cidade,Numero,Telefone,"+
            "AtendeSegunda,AtendeTerca,AtendeQuarta,AtendeQuinta,AtendeSexta,AtendeSabado,AtendeDomingo," +
            "HoraInicial,MinutoInicial,HoraFinal,MinutoFinal")] EspecialistaViewModel especialistaViewModel)
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
                        especialista.HoraInicial = especialistaViewModel.HoraInicial;
                        especialista.MinutoInicial = especialistaViewModel.MinutoInicial;
                        especialista.HoraFinal = especialistaViewModel.HoraFinal;
                        especialista.MinutoFinal = especialistaViewModel.MinutoFinal;
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
            ViewBag.Hora = GetListHour();
            ViewBag.Minuto = GetListMinute();
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
            "AtendeSegunda,AtendeTerca,AtendeQuarta,AtendeQuinta,AtendeSexta,AtendeSabado,AtendeDomingo,"+
            "HoraInicial,MinutoInicial,HoraFinal,MinutoFinal")] EspecialistaViewModel especialistaViewModel)
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
                            especialista.HoraInicial = especialistaViewModel.HoraInicial;
                            especialista.MinutoInicial = especialistaViewModel.MinutoInicial;
                            especialista.HoraFinal = especialistaViewModel.HoraFinal;
                            especialista.MinutoFinal = especialistaViewModel.MinutoFinal;
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

        public ActionResult Agenda()
        {
            ViewBag.especialistaId = new SelectList(db.Especialistas.ToList(), "Id", "Nome", null);
            return View(new List<AgendaViewModel>());
        }

        [MedicalFilter]
        public ActionResult Atendimento(int? id, string tipo)
        {

            if (id == null || string.IsNullOrEmpty(tipo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var atendimento = new AtendimentoViewModel();

            if (tipo == "Consulta")
            {
                var consulta = db.Consultas.Where(x => x.Id == id).Include(x => x.Paciente).Include(x => x.Especialista).Include(x => x.Especialidade).FirstOrDefault();
                atendimento.NomePaciente = consulta.Paciente.Nome;
                atendimento.Tipo = "Consulta";
                atendimento.DataNascimento = consulta.Paciente.DataNascimento;
                atendimento.NomeEspecialista = consulta.Especialista.Nome;
                atendimento.AnotacaoEspecialista = consulta.AnotacaoEspecialista;
                atendimento.IdAtendimento = int.Parse(id.ToString());
                atendimento.IdPaciente = consulta.Paciente.Id;
                atendimento.Especialidade = consulta.Especialidade.Nome;
                atendimento.Observacao = consulta.Observacao;
                atendimento.AnotacaoMedicamentos = consulta.Medicamentos;
            }

            if (tipo == "Procedimento")
            {

                var procedimento = db.Intervencoes.Where(x => x.Id == id).Include(x => x.Paciente).Include(x => x.Especialista).Include(x => x.Procedimento).FirstOrDefault();
                atendimento.NomePaciente = procedimento.Paciente.Nome;
                atendimento.DataNascimento = procedimento.Paciente.DataNascimento;
                atendimento.Tipo = "Procedimento";
                atendimento.NomeEspecialista = procedimento.Especialista.Nome;
                atendimento.AnotacaoEspecialista = procedimento.AnotacaoEspecialista;
                atendimento.IdAtendimento = int.Parse(id.ToString());
                atendimento.IdPaciente = procedimento.Paciente.Id;
                atendimento.Especialidade = procedimento.Procedimento.Nome;
                atendimento.Observacao = procedimento.Observacao;
                atendimento.AnotacaoMedicamentos = procedimento.Medicamentos;
            }

            var anamneses = db.ModelosAnamneses.ToList();

            var modelos = new SelectListItem();

            List<SelectListItem> modeloAnamneses = new List<SelectListItem>();

            modeloAnamneses.Add(new SelectListItem() { Text = "Selecione um Modelo Anamnese", Value = "-1" });
            foreach (ModeloAnamnese anamnese in anamneses)
            {
                modeloAnamneses.Add(new SelectListItem() { Text = anamnese.Nome, Value = anamnese.Id.ToString() });
            }

            ViewBag.ModeloAnamneses = modeloAnamneses;

            if (atendimento == null)
            {
                return HttpNotFound();
            }
            return View(atendimento);
        }

        [HttpPost]
        public ActionResult Agenda(string dataInicio, string dataFim, int especialistaId)
        {
            var especialist = db.Especialistas.ToList();

            var consultas = db.Consultas.Include(x => x.Especialidade).Include(x => x.Especialista).
                Include(x => x.Paciente).ToList();
            var procedimentos = db.Intervencoes.Include(x => x.Procedimento).Include(x => x.Paciente)
                .Include(x => x.Especialista).ToList();
            var prontuarios = new List<AgendaViewModel>();

            if (!string.IsNullOrEmpty(dataInicio))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(dataInicio, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date >= lvDateTime).ToList();
                    procedimentos = procedimentos.Where(x => x.Date >= lvDateTime).ToList();
                    ViewBag.dataInicio = dataInicio;
                }
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                DateTime lvDateTime = DateTime.MinValue;

                if (DateTime.TryParse(dataFim, out lvDateTime))
                {
                    consultas = consultas.Where(x => x.Date <= lvDateTime).ToList();
                    procedimentos = procedimentos.Where(x => x.Date <= lvDateTime).ToList();
                    ViewBag.dataInicio = dataInicio;
                }
            }

            if (especialistaId > 0)
            {
                consultas = consultas.Where(x => x.EspecialistaId == especialistaId).OrderByDescending(x => x.Date).ToList();
                procedimentos = procedimentos.Where(x => x.EspecialistaId == especialistaId).OrderByDescending(x => x.Date).ToList();
                ViewBag.especialistaId = new SelectList(db.Especialistas.ToList(), "Id", "Nome", especialistaId);
            }

            consultas.ForEach(x =>
            {
                prontuarios.Add(new AgendaViewModel()
                {
                    Tipo = "Consulta",
                    Data = x.Date,
                    Hora = x.Hora,
                    Minuto = x.Minuto,
                    Doutor = x.Especialista.Nome,
                    Especialidade = x.Especialidade.Nome,
                    NomePaciente = x.Paciente.Nome,
                    Observacao = x.Observacao,
                    Id = x.Id,
                    AtendimentoRealizado = x.AtendimentoRealizado
                });
            });

            procedimentos.ForEach(x =>
            {
                prontuarios.Add(new AgendaViewModel()
                {
                    Tipo = "Procedimento",
                    Data = x.Date,
                    Hora = x.Hora,
                    Minuto = x.Minuto,
                    Especialidade = x.Procedimento.Nome,
                    Doutor = x.Especialista.Nome,
                    NomePaciente = x.Paciente.Nome,
                    Observacao = x.Observacao,
                    Id = x.Id,
                    AtendimentoRealizado = x.AtendimentoRealizado
                });
            });

            prontuarios = prontuarios.OrderByDescending(x => x.Data).OrderByDescending(x => x.Hora).ToList();

            return View(prontuarios);
        }

        [HttpPost]
        public ActionResult GetEspecialistasById(int especialidadeId)
        {
            List<EspecialistaEspecialidade> especialistaEspecialidade = db.EspecialistasEspecialidades.Where(x => x.EspecialidadeId == especialidadeId).Include(x => x.Especialista).ToList();
            return Json(especialistaEspecialidade);
        }

        [HttpPost]
        public ActionResult VerifyDoctorTime(string data, int hora, int minuto, int doutorId)
        {
            var especialista = db.Especialistas.Where(x => x.Id == doutorId).FirstOrDefault();

            if (especialista != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    DateTime lvDateTime = DateTime.MinValue;

                    if (DateTime.TryParse(data, out lvDateTime))
                    {
                        var diasAtendimento = db.DiasAtendimentos.Where(x => x.Id == especialista.DiasAtendimentoId).FirstOrDefault();

                        if (diasAtendimento != null)
                        {
                            if (lvDateTime.DayOfWeek == DayOfWeek.Monday && !diasAtendimento.AtendeSegunda)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Tuesday && !diasAtendimento.AtendeTerca)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Wednesday && !diasAtendimento.AtendeQuarta)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Thursday && !diasAtendimento.AtendeQuinta)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Friday && !diasAtendimento.AtendeSexta)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Saturday && !diasAtendimento.AtendeSabado)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });
                            else if (lvDateTime.DayOfWeek == DayOfWeek.Sunday && !diasAtendimento.AtendeDomingo)
                                return Json(new object[] { false, "O médico não atende no dia selecionado!" });

                            if (hora >= especialista.HoraInicial && hora <= especialista.HoraFinal)
                            {
                                var consultas = new List<Consulta>();

                                foreach (var c in db.Consultas.Where(y => y.EspecialistaId == especialista.Id && y.Date == lvDateTime).ToList())
                                {
                                    if (Utility.TimeConflict(c.Date, c.Date, c.Hora, hora, c.Minuto, minuto))
                                    {
                                        consultas.Add(c);
                                        break;
                                    }
                                }

                                var procedimentos = new List<Intervencao>();

                                foreach (var p in db.Intervencoes.Where(y => y.EspecialistaId == especialista.Id && y.Date == lvDateTime).ToList())
                                {
                                    if (Utility.TimeConflict(p.Date, p.Date, p.Hora, hora, p.Minuto, minuto))
                                    {
                                        procedimentos.Add(p);
                                        break;
                                    }
                                }

                                if (consultas.Count() > 0 || procedimentos.Count() > 0)
                                    return Json(new object[] { false, "Possível conflito no horário do médico selecionado! <br/> "+
                                        "Por favor, verifique a agenda do especialista antes de marcar a consulta!" });

                                if (hora == especialista.HoraInicial && minuto >= especialista.MinutoInicial)
                                    return Json(new object[] { true, "" });
                                else if (hora > especialista.HoraInicial && hora < especialista.HoraFinal)
                                    return Json(new object[] { true, "" });
                                else if (hora == especialista.HoraFinal && minuto <= especialista.MinutoFinal)
                                    return Json(new object[] { true, "" });
                                else
                                    return Json(new object[] { false, "O médico não atende no horário selecionado! <br/>" +
                                        "Por favor, verifique a agenda do especialista antes de marcar a consulta! " });
                            }
                            else if (hora < especialista.HoraInicial || hora > especialista.HoraFinal)
                            {
                                return Json(new object[] { false, "O médico não atende no horário selecionado! <br/>" +
                                        "Por favor, verifique a agenda do especialista antes de marcar a consulta! " });
                            }

                        }
                        else
                        {
                            return Json(new object[] { false, "Não foi possível encontrar o horário do especialista! <br/>"+
                            " Por favor, verifique antes de consultar!" });
                        }

                    }
                    else
                    {
                        return Json(new object[] { false, "Informe uma data válida! <br/>" });
                    }
                }
                else
                {
                    return Json(new object[] { false, "Informe uma data válida! <br/>" });
                }
            }

            return Json(new object[] { false, "Não foi possível encontrar o horário do especialista! <br/>"+
                " Por favor, verifique antes de consultar!" });
        }

        [HttpPost]
        public ActionResult SalvarAtendimento(string anotacaoEspecialista, string anotacaoMedicamentos, int idAtendimento, string tipo)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (tipo == "Consulta")
                    {
                        var consulta = db.Consultas.Where(x => x.Id == idAtendimento).Include(x => x.Paciente).
                            Include(x => x.Especialidade).Include(x => x.Especialista).FirstOrDefault();
                        consulta.AnotacaoEspecialista = anotacaoEspecialista;
                        consulta.Medicamentos = anotacaoMedicamentos;
                        consulta.AtendimentoRealizado = true;
                    }
                    else if (tipo == "Procedimento")
                    {
                        var procedimento = db.Intervencoes.Where(x => x.Id == idAtendimento).Include(x => x.Paciente).
                            Include(x => x.Procedimento).Include(x => x.Especialista).FirstOrDefault();
                        procedimento.AnotacaoEspecialista = anotacaoEspecialista;
                        procedimento.Medicamentos = anotacaoMedicamentos;
                        procedimento.AtendimentoRealizado = true;
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    DebugLog.Logar(e.StackTrace);
                }
            }
            return RedirectToAction("Agenda");

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
