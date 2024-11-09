using EstateProManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace EstateProManager.Controllers
{
    public class ClientController : Controller
    {
        private readonly EstateProManagerContext _db;

        public ClientController(EstateProManagerContext db)
        {
            _db = db;
        }

        private bool checkUserProperty(long id)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("ID"));

            bool isPropertyInSales = _db.Ventes
                .Any(v => v.IdAcheteur == userId && v.Id == id);

            bool isPropertyInContracts = _db.Contrats
                .Any(c => c.IdLocataire == userId && c.Id == id);

            return isPropertyInSales || isPropertyInContracts;
        }
        private bool CheckAndSetUserData(HttpContext hc)
        {
            if (!Utility.IsUserLoggedIn(hc)) return false;

            if (!Convert.ToInt32(HttpContext.Session.GetString("IDROLE")).Equals(Utility.RoleClient)) return false;

            ViewBag.ID = HttpContext.Session.GetString("ID");
            ViewBag.CIN = HttpContext.Session.GetString("CIN");
            ViewBag.NOM = HttpContext.Session.GetString("NOM");
            ViewBag.PRENOM = HttpContext.Session.GetString("PRENOM");
            ViewBag.EMAIL = HttpContext.Session.GetString("EMAIL");
            ViewBag.DATENAISSANCE = HttpContext.Session.GetString("DATENAISSANCE");
            ViewBag.NUMEROTEL = HttpContext.Session.GetString("NUMEROTEL");
            ViewBag.ADRESSE = HttpContext.Session.GetString("ADRESSE");
            ViewBag.IDROLE = HttpContext.Session.GetString("IDROLE");
            ViewBag.LIBELLEROLE = HttpContext.Session.GetString("LIBELLEROLE");

            return true;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return RedirectToAction("SignIn", "EstatePro");
                }

                ViewBag.NbProprieteBuyed = _db.Ventes.Where(v => v.IdAcheteur.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID")))).Count();

                DateTime now = DateTime.Now;

                DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);

                ViewBag.NbProprieteLocation = _db.Contrats
                    .Join(_db.Utilisateurs, c => c.IdLocataire, u => u.Id, (c, u) => new { Contrat = c, Utilisateur = u })
                    .Where(joinResult => joinResult.Utilisateur.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID"))) && todayDate <= joinResult.Contrat.DateFin)
                    .Select(joinResult => joinResult.Contrat)
                    .Distinct()
                    .Count();

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

        [HttpGet]
        public IActionResult MyProperties()
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return RedirectToAction("SignIn", "EstatePro");
                }

                dynamic mymodels = new ExpandoObject();

                mymodels.clientPropertiesAcheter = _db.Ventes.Include(v => v.IdAcheteurNavigation)
                                                      .Include(v => v.IdVendeurNavigation)
                                                      .Include(V => V.IdProprieteNavigation)
                                                      .Where(v => v.IdAcheteur.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID"))))
                                                      .ToArray();

                mymodels.clientProprieteLocation = _db.Contrats.Include(c => c.IdLocataireNavigation)
                                                               .Include(c => c.IdProprieteNavigation)
                                                               .Where(c => c.IdLocataire.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID"))))
                                                               .ToArray();



                return View(mymodels);
            }
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }




        [HttpGet]
        public IActionResult MyAccount()
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return RedirectToAction("SignIn", "EstatePro");
                }

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

        [HttpPut]
        public JsonResult UpdateAccount([FromBody] Utilisateur U)
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter pour accéder à cette fonctionnalité." });
                }

                var existingUser = _db.Utilisateurs.Where(u => u.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID")))).FirstOrDefault();

                if (existingUser == null)
                {
                    return Json(new { success = false, message = "Impossible de modifier votre compte" });
                }

                existingUser.Cin = U.Cin;
                existingUser.Nom = U.Nom;
                existingUser.Prenom = U.Prenom;
                existingUser.Email = U.Email;
                existingUser.Birthdate = U.Birthdate;
                existingUser.NumeroDeTelephone = U.NumeroDeTelephone;
                existingUser.AdressePostale = U.AdressePostale;
                _db.SaveChanges();

                HttpContext.Session.SetString("ID", existingUser.Id.ToString());
                HttpContext.Session.SetString("CIN", existingUser.Cin.ToString());
                HttpContext.Session.SetString("NOM", existingUser.Nom.ToString());
                HttpContext.Session.SetString("PRENOM", existingUser.Prenom.ToString());
                HttpContext.Session.SetString("EMAIL", existingUser.Email.ToString());
                HttpContext.Session.SetString("DATENAISSANCE", existingUser.Birthdate.ToString());
                HttpContext.Session.SetString("NUMEROTEL", existingUser.NumeroDeTelephone.ToString());
                HttpContext.Session.SetString("ADRESSE", existingUser.AdressePostale.ToString());

                return Json(new { success = true, message = "Modification réussie" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }

        


        [HttpPost]
        public JsonResult CreateDemande([FromBody] Demande D)
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
                }
                if (D == null)
                {
                    return Json(new { success = false, message = "Informations invalides." });
                }
                if (!checkUserProperty(D.IdPropriete))
                {
                    return Json(new { success = false, message = "Ceci n'est pas votre propriete." });
                }

                

                D.IdUtilisateur = Convert.ToInt32(HttpContext.Session.GetString("ID")?.ToString());

                _db.Add(D);
                _db.SaveChanges();

                var generatedId = D.Id;

                return Json(new { success = true, message = "Création de la demande réussie.", generatedID = generatedId });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }
        [HttpPost]
        public JsonResult CreateTache(int id, [FromBody] Tach T)
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
                }
                if (T == null)
                {
                    return Json(new { success = false, message = "Informations invalides." });
                }
                if (!checkUserProperty(id))
                {
                    return Json(new { success = false, message = "Ceci n'est pas votre propriete." });
                }

                T.IdPrestataire = Convert.ToInt32(HttpContext.Session.GetString("ID")?.ToString());
                T.StatutTache = "En attente";

                _db.Add(T);
                _db.SaveChanges();

                var generatedId = T.Id;

                return Json(new { success = true, message = "Création de la tache réussie." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }



        [HttpGet]
        public IActionResult MyDemandes()
        {
            if (!CheckAndSetUserData(HttpContext))
            {
                return RedirectToAction("SignIn", "EstatePro");
            }

            dynamic mymodels = new ExpandoObject();
            mymodels.demands = _db.Demandes.Include(d => d.IdProprieteNavigation)
                                            .Include(d => d.IdUtilisateurNavigation)
                                            .Where(d => d.IdUtilisateur.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID")))).ToArray();

            return View(mymodels);
        }
        [HttpGet]
        public IActionResult MyDemandTache(int id)
        {
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return RedirectToAction("SignIn", "EstatePro");
                }

                // FOR SECURITY WE NEED TO CHECK IF IT'S HIS REAL TACHE !!!!! LIKE WE DID IN THE checkUserProperty(long id);

                dynamic mymodels = new ExpandoObject();
                mymodels.taches = _db.Taches.Include(t => t.IdDemandeNavigation)
                                            .Include(t => t.IdPrestataireNavigation)
                                            .Where(t => t.IdDemande.Equals(id)).ToArray();

                return View(mymodels);
            }
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

        [HttpGet]
        public IActionResult Diconnect()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("SignIn", "EstatePro");
            }
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }
    }
}
