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
    public class SousFamilleController : Controller
    {
        // GET: SousFamille
        BsDBEntities27 db3 = new BsDBEntities27();

        public ActionResult sousfamille()
        {
            List<tbl_famille> libelleList = db3.tbl_famille.Where(f => f.isDeleted == false).ToList();
            var libelleSelectList = libelleList.Select(d => new
            {
                idFamille = d.idFamille,
                displayText = d.nomFamille
            }).ToList();

            ViewBag.ListOfLibelle = new SelectList(libelleSelectList, "idFamille", "displayText");


            return View();
        }


        public JsonResult GetSousFamilleList(int page = 1, int pageSize = 5)
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

                var sousFamilleList = db3.tbl_sousFamille
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idSousFamille) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new sousFamille
                    {
                        idSousFamille = x.idSousFamille,
                        nomFamille = x.tbl_famille.nomFamille,
                        codeNom = x.codeNom,
                        nomSousFamille = x.nomSousFamille,

                        valeur = x.valeur,
                        unité = x.unité,

                        max=x.max,
                        unitéMax=x.unitéMax,
                        obsSousFamille=x.obsSousFamille,

                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db3.tbl_sousFamille.Count(x => x.isDeleted == false);

                return Json(sousFamilleList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }

        public JsonResult GetSousFamilleList2(int page = 1, int pageSize = 5)
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

                var sousFamilleList = db3.tbl_sousFamille
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idSousFamille) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new sousFamille
                    {
                        idSousFamille = x.idSousFamille,
                        nomFamille = x.tbl_famille.nomFamille,
                        codeNom = x.codeNom,
                        nomSousFamille = x.nomSousFamille,

                        valeur = x.valeur,
                        unité = x.unité,

                        max = x.max,
                        unitéMax = x.unitéMax,
                        obsSousFamille = x.obsSousFamille,

                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db3.tbl_sousFamille.Count(x => x.isDeleted == false);

                return Json( new { data = sousFamilleList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetSousFamilleById2(int idSousFamille)
        {
            tbl_sousFamille model = db3.tbl_sousFamille.Where(x => x.idSousFamille == idSousFamille).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSousFamilleById(int idSousFamille)
        {
            try
            {
                var model = db3.tbl_sousFamille
                    .Where(x => x.idSousFamille == idSousFamille && x.isDeleted == false)
                    .Select(x => new
                    {
                        idSousFamille = x.idSousFamille,
                        nomSousFamille=x.nomSousFamille,
                        valeur= x.valeur,
                        unité=x.unité,
                        max= x.max,
                        unitéMax=x.unitéMax,
                        obsSousFamille= x.obsSousFamille,
                        codeNom= x.codeNom,
                        idFamille=x.idFamille,


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




        [HttpPost]
        public JsonResult SaveDataInDatabase(sousFamille model)
        {
            var result = false;
            try
            {

                if (model.idSousFamille > 0)
                {
                    tbl_sousFamille sousfamille = db3.tbl_sousFamille.SingleOrDefault(x => x.isDeleted == false && x.idSousFamille == model.idSousFamille);
                    sousfamille.codeNom = model.codeNom;
                    sousfamille.nomSousFamille = model.nomSousFamille;
                    sousfamille.valeur = model.valeur;
                    sousfamille.unité = model.unité;
                    sousfamille.max = model.max;
                    sousfamille.unitéMax = model.unitéMax;
                    sousfamille.idFamille = model.idFamille;
                    sousfamille.obsSousFamille = model.obsSousFamille;
                    db3.SaveChanges();
                    result = true;
                }
                else
                {
                    tbl_sousFamille sousfamille = new tbl_sousFamille();
                    sousfamille.codeNom = model.codeNom;
                    sousfamille.nomSousFamille = model.nomSousFamille;
                    sousfamille.valeur = model.valeur;
                    sousfamille.unité = model.unité;
                    sousfamille.max = model.max;
                    sousfamille.unitéMax = model.unitéMax;
                    sousfamille.idFamille = model.idFamille;
                    sousfamille.obsSousFamille = model.obsSousFamille;
                    db3.tbl_sousFamille.Add(sousfamille);
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





        public JsonResult DeleteSousFamilleRecord(int idSousFamille) // Correction du nom du paramètre
        {
            bool result = false;
            tbl_sousFamille sousfamille = db3.tbl_sousFamille.SingleOrDefault(x => x.isDeleted == false && x.idSousFamille == idSousFamille);

            if (sousfamille != null)
            {
                sousfamille.isDeleted = true;
                db3.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
    }
}