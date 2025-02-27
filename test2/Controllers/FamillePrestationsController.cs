using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace test2.Controllers
{
    public class FamillePrestationsController : Controller
    {
        // GET: FamillePrestations
        BsDBEntities27 db3 = new BsDBEntities27();

        public ActionResult famille()
        {
            return View();
        }

        public JsonResult GetFamilleList(int page = 1, int pageSize = 5)
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

                var familleList = db3.tbl_famille
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idFamille) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new Famille
                    {
                        idFamille = x.idFamille,
                        nomFamille = x.nomFamille,
                        obsFamille = x.obsFamille
                        
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db3.tbl_famille.Count(x => x.isDeleted == false);

                return Json(familleList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }
        public JsonResult GetFamilleList2(int page = 1, int pageSize = 5)
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

                var familleList = db3.tbl_famille
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idFamille) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new Famille
                    {
                        idFamille = x.idFamille,
                        nomFamille = x.nomFamille,
                        obsFamille = x.obsFamille

                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db3.tbl_famille.Count(x => x.isDeleted == false);

                return Json(new { data = familleList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }
        [HttpGet]

        public JsonResult GetFamilleById3(int id)
        {
            tbl_famille model = db3.tbl_famille.Where(x => x.idFamille == id).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFamilleById(int id)
        {
            try
            {
                var model = db3.tbl_famille
                    .Where(x => x.idFamille == id && x.isDeleted == false)
                    .Select(x => new
                    {
                        idFamille = x.idFamille,
                        nomFamille = x.nomFamille,
                        obsFamille = x.obsFamille,
                      
                    })
                    .SingleOrDefault();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }

        public JsonResult SaveDataInDatabase(Famille model)
        {
            var result = false;
            try
            {

                if (model.idFamille > 0)
                {
                    tbl_famille famille = db3.tbl_famille.SingleOrDefault(x => x.isDeleted == false && x.idFamille == model.idFamille);
                    famille.nomFamille = model.nomFamille;
                    famille.obsFamille = model.obsFamille;
                    db3.SaveChanges();
                    result = true;
                }
                else
                {
                    tbl_famille famille = new tbl_famille();
                    famille.nomFamille = model.nomFamille;
                    famille.obsFamille = model.obsFamille;
                    db3.tbl_famille.Add(famille);
                    db3.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {
                // Gérer les exceptions ici
                return Json(new { success = false, errors = "Une erreur s'est produite lors de la sauvegarde des données." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFamilleRecord(int idFamille) // Correction du nom du paramètre
        {
            bool result = false;
            tbl_famille famille = db3.tbl_famille.SingleOrDefault(x => x.isDeleted == false && x.idFamille == idFamille);

            if (famille != null)
            {
                famille.isDeleted = true;
                db3.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}