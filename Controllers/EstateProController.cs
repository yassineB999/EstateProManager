using EstateProManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EstateProManager.Controllers
{
    public class EstateProController : Controller
    {
        private readonly EstateProManagerContext _db;

        public EstateProController(EstateProManagerContext db)
        {
            _db = db;
        }

        

        //                                           VIEWS                                       \\

        [HttpGet]
        public IActionResult Home()
        {
			try
			{
				return View();
			}
			catch (Exception ex)
			{
				return RedirectToAction("E404", "Error");
			}
		}

        [HttpGet]
        public IActionResult SignIn()
        {
            try
            {
                if (Utility.IsUserLoggedIn(HttpContext) && Convert.ToInt32(HttpContext.Session.GetString("IDROLE")).Equals(Utility.RoleAdministrator))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (Utility.IsUserLoggedIn(HttpContext) && Convert.ToInt32(HttpContext.Session.GetString("IDROLE")).Equals(Utility.RoleClient))
                {
                    return RedirectToAction("Dashboard", "Client");
                }
                else
                {
                    return View();
                }
			}
            catch (Exception ex)
            {
                return RedirectToAction("E404", "Error");
            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
			try
			{
				return View();
			}
			catch (Exception ex)
			{
				return RedirectToAction("E404", "Error");
			}
		}

        //                                         METHODS                                       \\

        public JsonResult SignIn([FromBody] Utilisateur U)
        {
            try
            {
                var user = _db.Utilisateurs
                       .Include(u => u.IdRoleNavigation)
                       .FirstOrDefault(us => us.Email == U.Email);

                if(user == null)
                {
                    return Json(new { success = false, message = "Compte introuvable" });
                }

                if (!BCrypt.Net.BCrypt.Verify(U.MotDePasse, user!.MotDePasse))
                {
                    return Json(new { success = false, message = "Identifiants invalides" });
                }

                var userRoleId = user.IdRoleNavigation.Id;

                HttpContext.Session.SetString("ID", user.Id.ToString());
                HttpContext.Session.SetString("CIN", user.Cin.ToString());
                HttpContext.Session.SetString("NOM", user.Nom.ToString());
                HttpContext.Session.SetString("PRENOM", user.Prenom.ToString());
                HttpContext.Session.SetString("EMAIL", user.Email.ToString());
                HttpContext.Session.SetString("DATENAISSANCE", user.Birthdate.ToString());
                HttpContext.Session.SetString("NUMEROTEL", user.NumeroDeTelephone.ToString());
                HttpContext.Session.SetString("ADRESSE", user.AdressePostale.ToString());
                HttpContext.Session.SetString("IDROLE", user.IdRoleNavigation.Id.ToString());
                HttpContext.Session.SetString("LIBELLEROLE", user.IdRoleNavigation.Libelle.ToString());


                return Json(new { success = true, message = "Connexion réussie", role = userRoleId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "E500" });
            }
        }

        public JsonResult SignUp([FromBody] Utilisateur U)
        {
            try
            {
                U.MotDePasse = BCrypt.Net.BCrypt.HashPassword(U.MotDePasse);
                U.IdRole = Utility.RoleClient;

                if (U == null)
                {
                    return Json(new { success = false, message = "Informations invalide" });
                }

                _db.Add(U);
                _db.SaveChanges();

                return Json(new { success = true, message = "Inscription réussie" });
			}
			catch (Exception ex)
			{
                return Json(new { success = false, error = "E500" });
            }
        }
    }
}
