using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace test2.Controllers
{
   
    public class AdhesionController : Controller
    {
        BsDBEntities27 db2 = new BsDBEntities27();

        // GET: Adhesion
        public ActionResult adhesionPersonnel()
        {
            return View();
        }
        public JsonResult GetAdherentList(int page = 1, int pageSize = 5)
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

                var adherentList = db2.tbl_adherent
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idAdherent) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new Adherent
                    {
                        idAdherent = x.idAdherent,
                        nomAdh = x.nomAdh,
                        prenomAdh = x.prenomAdh,
                        dateAdhesion = x.dateAdhesion,
                        status = x.status,
                        police = x.police,
                        benefEnCasDeces = x.benefEnCasDeces,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db2.tbl_adherent.Count(x => x.isDeleted == false);

                return Json(adherentList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }
        public JsonResult GetAdherentList2(int page = 1, int pageSize = 5)
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

                var adherentList = db2.tbl_adherent
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.idAdherent) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new Adherent
                    {
                        idAdherent = x.idAdherent,
                        nomAdh = x.nomAdh,
                        prenomAdh = x.prenomAdh,
                        dateAdhesion = x.dateAdhesion,
                        status = x.status,
                        police = x.police,
                        benefEnCasDeces = x.benefEnCasDeces,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db2.tbl_adherent.Count(x => x.isDeleted == false);

                return Json(new { data = adherentList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }
        public JsonResult GetAdherentById(int id)
        {
            try
            {
                var model = db2.tbl_adherent
                             .Where(x => x.idAdherent == id)
                             .Select(x => new
                             {
                                 idAdherent = x.idAdherent,
                                 nomAdh = x.nomAdh,
                                 prenomAdh = x.prenomAdh,
                                 CIN_PASS = x.CIN_PASS,
                                 adressAdh = x.adressAdh,
                                 emailAdh = x.emailAdh,
                                 matriculeCnam = x.matriculeCnam,
                                 status = x.status,
                                 numTelephone = x.numTelephone,
                                 isDeleted = x.isDeleted,
                                 police = x.police,
                                 dateAdhesion = x.dateAdhesion,
                                 dateNais = x.dateNais,
                                 lieuNais = x.lieuNais,
                                 sexeAdherent = x.sexeAdherent,
                                 situationFamiliale = x.situationFamiliale,
                                 gouvernorat = x.gouvernorat,
                                 codePostal = x.codePostal,
                                 profession = x.profession,
                                 lieu = x.lieu,
                                 salaireBrut = x.salaireBrut,
                                 RIB = x.RIB,
                                 benefEnCasDeces = x.benefEnCasDeces,
                             })
                             .SingleOrDefault();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }



        [HttpPost]
        public JsonResult SaveNewAdherent(Adherent model, List<Prestataire> conjoints , List<Prestataire> enfants)
        {
            var result = false;

            try
            {
                // Création d'un nouvel adhérent
                tbl_adherent adh = new tbl_adherent();
                {
                    // Création d'un nouvel adhérent
                    adh.idAdherent = model.idAdherent;
                    adh.nomAdh = model.nomAdh;
                    adh.prenomAdh = model.prenomAdh;
                    adh.CIN_PASS = model.CIN_PASS;
                    adh.adressAdh = model.adressAdh;
                    adh.emailAdh = model.emailAdh;
                    adh.numTelephone = model.numTelephone;
                    adh.gouvernorat = model.gouvernorat;
                    adh.codePostal = model.codePostal;
                    adh.dateNais = model.dateNais;
                    adh.lieuNais = model.lieuNais;
                    adh.profession = model.profession;
                    adh.lieu = model.lieu;
                    adh.sexeAdherent = model.sexeAdherent;
                    adh.situationFamiliale = model.situationFamiliale;
                    adh.RIB = model.RIB;
                    adh.salaireBrut = model.salaireBrut;
                    adh.benefEnCasDeces = model.benefEnCasDeces;
                    adh.police = model.police;
                    adh.matriculeCnam = model.matriculeCnam;
                    adh.dateAdhesion = model.dateAdhesion;
                    adh.status = model.status;
                    // Ajoutez les autres propriétés de l'adhérent ici
                    adh.isDeleted = false;

                };

                db2.tbl_adherent.Add(adh);

                // Ajout des conjoints
                if (conjoints != null && conjoints.Any())
                {
                    foreach (var conjoint in conjoints)
                    {
                        // Création d'un nouvel objet prestataire pour le conjoint
                        tbl_prestataire conjointEntity = new tbl_prestataire
                        {
                            codePrestataire = conjoint.codePrestataire,
                            nomPres = conjoint.nomPres,
                            prenomPres = conjoint.prenomPres,
                            dateNais = conjoint.dateNais,
                            sexe = conjoint.sexe,
                            profession = conjoint.profession,
                            employeur = conjoint.employeur,
                            idAdherent = model.idAdherent // Lien vers l'adhérent
                        };

                        db2.tbl_prestataire.Add(conjointEntity);
                    }
                }
                if (enfants != null && enfants.Any())
                {
                    foreach (var enfant in enfants)
                    {
                        // Création d'un nouvel objet prestataire pour l'enfant
                        tbl_prestataire enfantEntity = new tbl_prestataire
                        {
                            codePrestataire = enfant.codePrestataire,
                            prenomPres = enfant.prenomPres,
                            dateNais = enfant.dateNais,
                            sexe = enfant.sexe,
                            idAdherent = model.idAdherent // Lien vers l'adhérent
                        };

                        db2.tbl_prestataire.Add(enfantEntity);
                    }
                }


                db2.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                // Logguer l'exception pour le suivi
                // Gérer les exceptions ici
                return Json(new { success = false, errors = "Une erreur s'est produite lors de la sauvegarde des données." });
            }

           
            return Json(new { success = result}, JsonRequestBehavior.AllowGet);
      
        }

        // Méthode pour récupérer les conjoints en fonction de l'ID de l'adhérent
        [HttpGet]
        public ActionResult GetConjoints(int idAdherent)
        {
            try
            {
                // Récupérer les conjoints en fonction de l'ID de l'adhérent
                var prestataires = db2.tbl_prestataire
                    .Where(p => p.idAdherent == idAdherent && p.codePrestataire == "99")
                    .Select(p => new Prestataire
                    {
                        idPrestataire = p.idPrestataire,
                        codePrestataire=p.codePrestataire,
                        dateNais = p.dateNais,
                        nomPres = p.nomPres,
                        prenomPres = p.prenomPres,
                        profession = p.profession,
                        employeur = p.employeur,
                        sexe = p.sexe,
                        // Autres propriétés
                    })
                    .ToList();

                return Json(prestataires, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Gérer les erreurs et renvoyer un code d'erreur approprié
                return Json(new { error = "Une erreur s'est produite lors de la récupération des conjoints." });
            }
        }

        [HttpGet]
        public ActionResult GetEnfants(int idAdherent)
        {
            try
            {
                // Récupérer les enfants en fonction de l'ID de l'adhérent
                var prestatairesE = db2.tbl_prestataire
                    .Where(p => p.idAdherent == idAdherent && p.codePrestataire != "00" && p.codePrestataire != "99")
                    .Select(p => new Prestataire
                    {
                        idPrestataire = p.idPrestataire,
                        codePrestataire = p.codePrestataire,
                        dateNais = p.dateNais,                      
                        prenomPres = p.prenomPres,
                        sexe = p.sexe,
                        // Autres propriétés
                    })
                    .ToList();

                return Json(prestatairesE, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs et renvoyer un code d'erreur approprié
                return Json(new { error = "Une erreur s'est produite lors de la récupération des enfants." });
            }
        }



        [HttpPost]
        public JsonResult SaveAdherentData(Adherent model , List<Prestataire> conjoints, List<Prestataire> enfants)
        {
            var result = false;
            try
            {
                if (model.idAdherent > 0) // Modification d'un adhérent existant
                {
                    // Modification d'un adhérent existant
                    tbl_adherent adherent = db2.tbl_adherent.SingleOrDefault(x => x.isDeleted == false && x.idAdherent == model.idAdherent);
                    if (adherent != null)
                    {
                        adherent.nomAdh = model.nomAdh;
                        adherent.prenomAdh = model.prenomAdh;
                        adherent.CIN_PASS = model.CIN_PASS;
                        adherent.adressAdh = model.adressAdh;
                        adherent.emailAdh = model.emailAdh;
                        adherent.numTelephone = model.numTelephone;
                        adherent.gouvernorat = model.gouvernorat;
                        adherent.codePostal = model.codePostal;
                        adherent.dateNais = model.dateNais;
                        adherent.lieuNais = model.lieuNais;
                        adherent.profession = model.profession;
                        adherent.lieu = model.lieu;
                        adherent.sexeAdherent = model.sexeAdherent;
                        adherent.situationFamiliale = model.situationFamiliale;
                        adherent.RIB = model.RIB;
                        adherent.salaireBrut = model.salaireBrut;
                        adherent.benefEnCasDeces = model.benefEnCasDeces;
                        adherent.status = model.status;
                        adherent.dateAdhesion = model.dateAdhesion;
                        adherent.police = model.police;
                        adherent.matriculeCnam = model.matriculeCnam;


                    }
                    if (conjoints != null && conjoints.Any())
                    {
                        foreach (var conjoint in conjoints)
                        {
                            // Création d'un nouvel objet prestataire pour le conjoint
                            tbl_prestataire conjointEntity = new tbl_prestataire
                            {
                                codePrestataire = conjoint.codePrestataire,
                                nomPres = conjoint.nomPres,
                                prenomPres = conjoint.prenomPres,
                                dateNais = conjoint.dateNais,
                                sexe = conjoint.sexe,
                                profession = conjoint.profession,
                                employeur = conjoint.employeur,
                                idAdherent = model.idAdherent // Lien vers l'adhérent
                            };

                            db2.tbl_prestataire.Add(conjointEntity);
                        }
                    }
                    if (enfants != null && enfants.Any())
                    {
                        foreach (var enfant in enfants)
                        {
                            // Création d'un nouvel objet prestataire pour l'enfant
                            tbl_prestataire enfantEntity = new tbl_prestataire
                            {
                                codePrestataire = enfant.codePrestataire,
                                prenomPres = enfant.prenomPres,
                                dateNais = enfant.dateNais,
                                sexe = enfant.sexe,
                                idAdherent = model.idAdherent // Lien vers l'adhérent
                            };

                            db2.tbl_prestataire.Add(enfantEntity);
                        }
                    }

                    // Ajoutez les autres propriétés de l'adhérent ici

                    db2.SaveChanges();
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

        [HttpPost]

        public JsonResult DeleteAdherentRecord(int idAdherent)
        {
            bool result = false;
            tbl_adherent adherent = db2.tbl_adherent.SingleOrDefault(x => x.isDeleted == false && x.idAdherent == idAdherent);
            if (adherent != null)
            {
                adherent.isDeleted = true;
                db2.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPresById(int idAdherent)
        {
            try
            {
                var prestataires = db2.tbl_prestataire
                    .Where(p => p.idAdherent == idAdherent)
                    .Select(p => new Prestataire
                    {
                        idPrestataire = p.idPrestataire,
                        dateNais = p.dateNais,
                        nomPres = p.nomPres,
                        prenomPres = p.prenomPres,
                        profession = p.profession,
                        employeur = p.employeur,
                        // Autres propriétés
                    })
                    .ToList();
                return Json(prestataires, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Une erreur s'est produite lors de la récupération des informations du prestataire." });
            }
        }

        [HttpPost]
        public ActionResult DeleteConjoint(int idPrestataire)
        {
            try
            {

                // Trouver la rubrique dans la base de données
                var conjointToDelete = db2.tbl_prestataire.Find(idPrestataire);
                if (conjointToDelete != null)
                {
                    // Supprimer la rubrique de la base de données
                    db2.tbl_prestataire.Remove(conjointToDelete);
                    db2.SaveChanges();

                    return Json(new { success = true, message = "Rubrique supprimée avec succès de la base de données." });
                }
                else
                {
                    return Json(new { success = false, message = "Rubrique non trouvée dans la base de données." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la suppression de la rubrique de la base de données." });
            }
        }

        [HttpPost]
        public ActionResult DeleteEnfant(int idPrestataire)
        {
            try
            {
                // Trouver l'enfant dans la base de données
                var enfantToDelete = db2.tbl_prestataire.Find(idPrestataire);
                if (enfantToDelete != null)
                {
                    // Supprimer l'enfant de la base de données
                    db2.tbl_prestataire.Remove(enfantToDelete);
                    db2.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Enfant non trouvé dans la base de données." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la suppression de l'enfant de la base de données." });
            }
        }



        public ActionResult FilterByAdherentId(int idAdherent)
        {
            try
            {
                var filteredAdhList = db2.tbl_adherent
                    .Where(x => x.isDeleted == false && x.idAdherent == idAdherent)
                    .Select(x => new Adherent
                    {
                        idAdherent = x.idAdherent,
                        nomAdh = x.nomAdh,
                        prenomAdh = x.prenomAdh,
                        dateAdhesion = x.dateAdhesion,
                        status = x.status,
                        police = x.police,
                        benefEnCasDeces = x.benefEnCasDeces,

                    }).ToList();

                return Json(filteredAdhList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs ici
                return Json(new { error = ex.Message });
            }
        }


    }
}
