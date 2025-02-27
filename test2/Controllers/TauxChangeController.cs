using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace test2.Controllers
{
    public class TauxChangeController : Controller
    {
        // GET: TauxChange
        BsDBEntities27 db1 = new BsDBEntities27();

        public ActionResult taux()
        {
            return View();
        }
        public JsonResult GetTCList(int page = 1, int pageSize = 5)
        {
            try
            {
                // Validation des paramètres
                if (page <= 0)
                {
                    page = 1; // Page par défaut si page est inférieur ou égal à zéro
                }

                if (pageSize <= 0)
                {
                    pageSize = 5; // Taille de page par défaut si pageSize est inférieur ou égal à zéro
                }

                var tauxList = db1.tbl_taux_de_change
                    .Where(x => x.is_deleted == false)
                    .OrderBy(x => x.id) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new tauxChange
                    {
                        id = x.id,
                        devise_locale = x.devise_locale,
                        devise_etrangere = x.devise_etrangere,
                        date_debut = x.date_debut,
                        date_fin = x.date_fin,
                        taux = x.taux,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db1.tbl_taux_de_change.Count(x => x.is_deleted == false);

                return Json(tauxList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetTCList2(int page = 1, int pageSize = 5)
        {
            try
            {
                // Validation des paramètres
                if (page <= 0)
                {
                    page = 1; // Page par défaut si page est inférieur ou égal à zéro
                }

                if (pageSize <= 0)
                {
                    pageSize = 5; // Taille de page par défaut si pageSize est inférieur ou égal à zéro
                }

                var tauxList = db1.tbl_taux_de_change
                    .Where(x => x.is_deleted == false)
                    .OrderBy(x => x.id) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new tauxChange
                    {
                        id = x.id,
                        devise_locale = x.devise_locale,
                        devise_etrangere = x.devise_etrangere,
                        date_debut = x.date_debut,
                        date_fin = x.date_fin,
                        taux = x.taux,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db1.tbl_taux_de_change.Count(x => x.is_deleted == false);

                return Json( new { data = tauxList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetTauxById(int id)
        {
            tbl_taux_de_change model = db1.tbl_taux_de_change.Where(x => x.id == id).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDataInDatabase(tauxChange model)
        {
            var result = false;
            try
            {
                if (model.taux != 0) // Check if the "Taux" field is not null
                {
                    if (model.id > 0)
                    {
                        tbl_taux_de_change taux = db1.tbl_taux_de_change.SingleOrDefault(x => x.is_deleted == false && x.id == model.id);
                        taux.devise_locale = model.devise_locale;
                        taux.devise_etrangere = model.devise_etrangere;
                        taux.date_debut = model.date_debut;
                        taux.date_fin = model.date_fin;
                        taux.taux = model.taux;

                        db1.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        tbl_taux_de_change taux = new tbl_taux_de_change();
                        taux.devise_locale = model.devise_locale;
                        taux.devise_etrangere = model.devise_etrangere;
                        taux.date_debut = model.date_debut;
                        taux.date_fin = model.date_fin;
                        taux.taux = model.taux;
                        db1.tbl_taux_de_change.Add(taux);
                        db1.SaveChanges();
                        result = true;
                    }
                }
                else
                {
                    // If the "Taux" field is null, return an error message
                    return Json(new { success = false, errors = "La valeur du taux ne peut pas être nulle." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return Json(new { success = false, errors = "Une erreur s'est produite lors de la sauvegarde des données." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteTauxRecord(int id)
        {
            bool result = false;
            tbl_taux_de_change taux = db1.tbl_taux_de_change.SingleOrDefault(x => x.is_deleted == false && x.id == id);
            if (taux != null)
            {
                taux.is_deleted = true;
                db1.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}