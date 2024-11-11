using EstateProManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace EstateProManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly EstateProManagerContext _db;

        public AdminController(EstateProManagerContext db)
        {
            _db = db;
        }

        private bool CheckAndSetUserData(HttpContext hc)
        {
            if (!Utility.IsUserLoggedIn(hc)) return false;

            if (!Convert.ToInt32(HttpContext.Session.GetString("IDROLE")).Equals(Utility.RoleAdministrator)) return false;

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

        //                                           VIEWS                                       \\
        [HttpGet]
        public IActionResult Dashboard()
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				ViewBag.NbBuyers = _db.Ventes.Count(v => !v.IdAcheteur.Equals(null));
				ViewBag.NbTenants = _db.Contrats.Count(c => !c.IdLocataire.Equals(null));

				ViewBag.NbProperties = _db.Proprietes.Count();
				ViewBag.NbUsers = _db.Utilisateurs.Count();

				return View();
			}
            catch (Exception ex)
            {
				return RedirectToAction("E404", "Error");
			}
		}




        [HttpGet]
        public IActionResult ManageUsers()
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				dynamic mymodels = new ExpandoObject();
				mymodels.users = _db.Utilisateurs.Include(u => u.IdRoleNavigation).Where(u => !u.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("ID")))).ToArray();
				mymodels.roles = _db.Roles.ToArray();

				return View(mymodels);
			}
            catch (Exception ex)
            {
				return RedirectToAction("E404", "Error");
			}
        }

        [HttpPost]
        public JsonResult CreateUser([FromBody] Utilisateur U)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (U == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				string generated_Password = Utility.GeneratePassword();

				//Utility.SendEmail(U.Email, "EstateProManager Mot De Passe", "Bienvenue " + U.Prenom + " " + U.Nom + " dans notre application voici votre identifiant : \n Email : " + U.Email + " \n Mot De Passe : " + generated_Password);

				U.MotDePasse = BCrypt.Net.BCrypt.HashPassword(generated_Password);

				_db.Add(U);
				_db.SaveChanges();

				var generatedId = U.Id;

				return Json(new { success = true, message = "Création d'utilisateur réussie.", generatedID = generatedId });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}

        [HttpPut]
        public JsonResult UpdateUser([FromBody] Utilisateur U)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (U == null)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}

				var existingUser = _db.Utilisateurs.Where(u => u.Id.Equals(U.Id)).FirstOrDefault();

				if (existingUser == null)
				{
					return Json(new { success = false, message = "Utilisateur introuvable" });
				}

				existingUser.Cin = U.Cin;
				existingUser.Nom = U.Nom;
				existingUser.Prenom = U.Prenom;
				existingUser.Email = U.Email;
				existingUser.Birthdate = U.Birthdate;
				existingUser.NumeroDeTelephone = U.NumeroDeTelephone;
				existingUser.AdressePostale = U.AdressePostale;
				existingUser.IdRole = U.IdRole;
				_db.SaveChanges();

				return Json(new { success = true, message = "Modification de l'utilisateur réussie" });
			}
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }

        [HttpDelete]
        public JsonResult DeleteUser(int id)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}
				Utilisateur U = new Utilisateur();
				U.Id = id;

				_db.Utilisateurs.Remove(U);
				_db.SaveChanges();
				return Json(new { success = true, message = "Utilisateur supprimé" });
			}
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }




        [HttpGet]
        public IActionResult ManageProperties()
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				dynamic mymodels = new ExpandoObject();
				mymodels.proprietes = _db.Proprietes.ToArray();
				return View(mymodels);
			}
            catch (Exception ex)
            {
				return RedirectToAction("E404", "Error");
			}
        }

        [HttpGet]
        public IActionResult ManageProperty(int id)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				var property = _db.Proprietes.Where(p => p.Id == id).FirstOrDefault();

				if (property == null)
				{
					return RedirectToAction("ManageProperties");
				}

				ViewBag.ID = property.Id;
				ViewBag.TYPE = property.Type;
				ViewBag.TAILLE = property.Taille;
				ViewBag.EMPLACEMENT = property.Emplacement;
				ViewBag.CARACTERISTIQUES = property.Caracteristiques;
				ViewBag.IMAGES = property.Images;
				ViewBag.DOCUMENTS = property.Documents;
				ViewBag.VALEUR = property.Valeur;
				ViewBag.DATECONSTRUCTION = property.DateDeConstruction;

				return View();
			}
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

		public IActionResult DownloadImage(int id)
		{
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				var property = _db.Proprietes.FirstOrDefault(p => p.Id == id);

				if (property == null)
				{
					return RedirectToAction("ManageProperties");
				}

				if (string.IsNullOrEmpty(property.Images))
				{
					return NotFound();
				}

				byte[] imageData = Convert.FromBase64String(property.Images.Trim());
				return File(imageData, "image/jpeg", "image.jpg");
			}
            catch (Exception ex)
            {
                return RedirectToAction("E500", "Error");
            }
		}


		public IActionResult DownloadDocument(int id)
		{
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				var property = _db.Proprietes.FirstOrDefault(p => p.Id == id);

				if (property == null)
				{
					return RedirectToAction("ManageProperties");
				}

				if (string.IsNullOrEmpty(property.Documents))
				{
					return NotFound();
				}

				byte[] documentData = Convert.FromBase64String(property.Documents.Trim());
				return File(documentData, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "document.docx");
			}
            catch (Exception ex)
            {
				return RedirectToAction("E500", "Error");
			}
		}


		[HttpPost]
        public JsonResult CreateProperty([FromBody] Propriete P)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (P == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				_db.Add(P);
				_db.SaveChanges();

				var generatedId = P.Id;

				return Json(new { success = true, message = "Création de propriete réussie.", generatedID = generatedId });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}

        [HttpPut]
        public JsonResult UpdateProperty([FromBody] Propriete P)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (P == null)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}

				var existingProperty = _db.Proprietes.Find(P.Id);

				if (existingProperty == null)
				{
					return Json(new { success = false, message = "Propriété introuvable" });
				}

				existingProperty.Type = P.Type;
				existingProperty.Taille = P.Taille;
				existingProperty.Emplacement = P.Emplacement;
				existingProperty.Caracteristiques = P.Caracteristiques;
				existingProperty.Valeur = P.Valeur;
				existingProperty.DateDeConstruction = P.DateDeConstruction;

				_db.SaveChanges();

				return Json(new { success = true, message = "Modification de la propriété réussie" });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}

        [HttpDelete]
        public JsonResult DeleteProperty(int id)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}

				Propriete P = new Propriete();
				P.Id = id;
				_db.Proprietes.Remove(P);
				_db.SaveChanges();

				return Json(new { success = true, message = "Propriété supprimée" });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}




        [HttpGet]
        public IActionResult ManageBuyers()
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				dynamic mymodels = new ExpandoObject();
				mymodels.ventes = _db.Ventes.Include(v => v.IdProprieteNavigation)
											.Include(v => v.IdAcheteurNavigation)
											.Include(v => v.IdVendeurNavigation)
											.ToArray();

				mymodels.properties = _db.Proprietes
					.Where(p => !_db.Ventes.Any(v => v.IdPropriete == p.Id) &&
								!_db.Contrats.Any(c => c.IdPropriete == p.Id)).ToArray();

				mymodels.utilisateurs = _db.Utilisateurs.ToArray();


				return View(mymodels);
			}
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

        [HttpPost]
        public JsonResult CreateBuyer([FromBody] Vente V)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (V == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				_db.Add(V);
				_db.SaveChanges();

				var generatedId = V.Id;

				return Json(new { success = true, message = "Création de vente réussie.", generatedID = generatedId });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}

        [HttpPut]
        public JsonResult UpdateBuyer([FromBody] Vente V)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (V == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				var existingVente = _db.Ventes.Find(V.Id);

				if (existingVente == null)
				{
					return Json(new { success = false, message = "Vente introuvable" });
				}

				existingVente.IdAcheteur = V.IdAcheteur;
				existingVente.IdVendeur = V.IdVendeur;
				existingVente.PrixVente = V.PrixVente;
				existingVente.DateVente = V.DateVente;

				_db.SaveChanges();

				return Json(new { success = true, message = "Modification de la vente réussie" });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
        }

        [HttpDelete]
        public JsonResult DeleteBuyer(int id)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}
				Vente V = new Vente();
				V.Id = id;

				_db.Ventes.Remove(V);
				_db.SaveChanges();

				return Json(new { success = true, message = "Ventes supprimée" });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
		}




        [HttpGet]
        public IActionResult ManageRentals()
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				dynamic mymodels = new ExpandoObject();
				mymodels.contrats = _db.Contrats.Include(v => v.IdProprieteNavigation)
											.Include(v => v.IdLocataireNavigation)
											.ToArray();

				mymodels.properties = _db.Proprietes
					.Where(p => !_db.Ventes.Any(v => v.IdPropriete == p.Id) &&
								!_db.Contrats.Any(c => c.IdPropriete == p.Id)).ToArray();

				mymodels.utilisateurs = _db.Utilisateurs.ToArray();

				return View(mymodels);
			}
            catch (Exception ex)
            {
				return RedirectToAction("E404", "Error");
			}
        }

        [HttpPost]
        public JsonResult CreateRental([FromBody] Contrat C)
        {
            try
            {
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}
				if (C == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				_db.Add(C);
				_db.SaveChanges();

				var generatedId = C.Id;

				return Json(new { success = true, message = "Création de contrat de location réussie.", generatedID = generatedId });
			}
            catch (Exception ex)
            {
				return Json(new { success = false, error = "E500" });
			}
        }

        [HttpPut]
        public JsonResult UpdateRental([FromBody] Contrat C)
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (C == null)
				{
					return Json(new { success = false, message = "Informations invalides." });
				}

				var existingRental = _db.Contrats.Find(C.Id);

				if (existingRental == null)
				{
					return Json(new { success = false, message = "Contrat de location introuvable" });
				}

				existingRental.IdPropriete = C.IdPropriete;
				existingRental.IdLocataire = C.IdLocataire;
				existingRental.DateDebut = C.DateDebut;
				existingRental.DateFin = C.DateFin;
				existingRental.PaiementMensuel = C.PaiementMensuel;

				_db.SaveChanges();

				return Json(new { success = true, message = "Modification du contrat de location réussie" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = "E500" });
			}
        }

        [HttpDelete]
        public JsonResult DeleteRental(int id)
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}
				Contrat C = new Contrat();
				C.Id = id;

				_db.Contrats.Remove(C);
				_db.SaveChanges();

				return Json(new { success = true, message = "Contrat location supprmée" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = "E500" });
			}
		}




        [HttpGet]
        public IActionResult ManageDemands()
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

				dynamic mymodels = new ExpandoObject();
				mymodels.demands = _db.Demandes.Include(d => d.IdProprieteNavigation)
												.Include(d => d.IdUtilisateurNavigation).ToArray();

				return View(mymodels);
			}
			catch (Exception ex)
			{
				return RedirectToAction("E404", "Error");
			}
        }

        [HttpGet]
        public IActionResult ManageDemandTache(int id)
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return RedirectToAction("SignIn", "EstatePro");
				}

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
		[HttpPost]
		public JsonResult UpdateEtatTache(int id)
		{
            try
            {
                if (!CheckAndSetUserData(HttpContext))
                {
                    return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
                }

                if (id <= 0)
                {
                    return Json(new { success = false, message = "Informations invalides" });
                }

				var existingTrache = _db.Taches.Where(t => t.Id.Equals(id)).FirstOrDefault();

				if (existingTrache == null)
				{
					return Json(new { success = false, message = "Tache introuvable" });
                }

				existingTrache.StatutTache = "Réparer";

                _db.SaveChanges();

                return Json(new { success = true, message = "Etat de la tache changer" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }

        [HttpDelete]
        public JsonResult DeleteDemand(int id)
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}
				Demande D = new Demande();
				D.Id = id;

				_db.Demandes.Remove(D);
				_db.SaveChanges();

				return Json(new { success = true, message = "Demande supprmée" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = "E500" });
			}
		}

        [HttpDelete]
        public JsonResult DeleteDemandTache(int id)
        {
			try
			{
				if (!CheckAndSetUserData(HttpContext))
				{
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
				}

				if (id <= 0)
				{
					return Json(new { success = false, message = "Informations invalides" });
				}
				Tach T = new Tach();
				T.Id = id;

				_db.Taches.Remove(T);
				_db.SaveChanges();

				return Json(new { success = true, message = "Tache supprmée" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = "E500" });
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
					return Json(new { success = false, message = "Permissions insuffisantes. Veuillez vous connecter en tant qu'administrateur pour accéder à cette fonctionnalité." });
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