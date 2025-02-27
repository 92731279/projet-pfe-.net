using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace test2.Controllers
{
    public class AdhAccountController : Controller
    {
        // GET: AdhAccount
        BsDBEntities25 db5 = new BsDBEntities25();

        public ActionResult IndexAdh()
        {
            // Récupérer la liste des prestataires
            List<tbl_adherent> adhList = db5.tbl_adherent.ToList();

            // Sélectionner distinctement les valeurs de codePrestataire
            var distinctAdherent = adhList.Select(d => d.idAdherent).Distinct().ToList();

            // Créer une liste d'objets anonymes contenant les valeurs idPrestataire et displayText (codePrestataire)
            var adhtSelectList = distinctAdherent.Select(idAdherent => new
            {
                idAdherent = adhList.FirstOrDefault(p => p.idAdherent == idAdherent)?.idAdherent, // Récupérer l'ID correspondant au codePrestataire
                displayText = idAdherent // Utiliser le codePrestataire comme texte d'affichage
            }).ToList();

            // Créer une SelectList à partir de la liste d'objets anonymes
            ViewBag.ListOfAdh = new SelectList(adhtSelectList, "idAdherent", "displayText");
            return View();
        }

        public ActionResult AdherentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Récupérer les informations de l'adhérent correspondant à l'ID spécifié
            tbl_adherent adherent = db5.tbl_adherent.FirstOrDefault(a => a.idAdherent == id);
            if (adherent == null)
            {
                return HttpNotFound();
            }

            // Retourner uniquement le nom et le prénom de l'adhérent sous forme de données JSON
            return Json(new { nomAdh = adherent.nomAdh, prenomAdh = adherent.prenomAdh }, JsonRequestBehavior.AllowGet);
        }

    }
}