using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using test2.Models;
using System.Globalization;
using CsQuery.EquationParser.Implementation;

namespace project.Controllers
{
    public class HomeController : Controller
    {
        BsDBEntities31 db = new BsDBEntities31();
        public ActionResult Index()
        {
            List<tbl_designation> DesigList = db.tbl_designation.ToList();
            var desigSelectList = DesigList.Select(d => new
            {
                designationID = d.designationId,
                displayText = d.designationNo + " - " + d.designationNom
            }).ToList();

            ViewBag.ListOfDesignation = new SelectList(desigSelectList, "designationID", "displayText");

            // -------------------------------------------------------------------------------------------------PrestList

            List<tbl_sousFamille> libelleList = db.tbl_sousFamille.Where(f => f.isDeleted == false).ToList();
            var prestationSelectList = libelleList.Select(d => new
            {
                idSousFamille = d.idSousFamille,
                displayText = d.codeNom + " - " + d.nomSousFamille
            }).ToList();

            ViewBag.ListOfPrestation = new SelectList(prestationSelectList, "idSousFamille", "displayText");

            //--------------------------------------------------------------------------------------------------------------

            // Récupérer la liste des prestataires
            List<tbl_prestataire> PrestList = db.tbl_prestataire.ToList();

            // Sélectionner distinctement les valeurs de codePrestataire
            var distinctPrestataires = PrestList.Select(d => d.codePrestataire).Distinct().ToList();

            // Créer une liste d'objets anonymes contenant les valeurs idPrestataire et displayText (codePrestataire)
            var prestSelectList = distinctPrestataires.Select(code => new
            {
                idPrestataire = PrestList.FirstOrDefault(p => p.codePrestataire == code)?.idPrestataire, // Récupérer l'ID correspondant au codePrestataire
                displayText = code // Utiliser le codePrestataire comme texte d'affichage
            }).ToList();

            // Créer une SelectList à partir de la liste d'objets anonymes
            ViewBag.ListOfPrestataire = new SelectList(prestSelectList, "idPrestataire", "displayText");

            return View();
        }
        public ActionResult index2()
        {
            List<tbl_designation> DesigList = db.tbl_designation.ToList();
            var desigSelectList = DesigList.Select(d => new
            {
                designationID = d.designationId,
                displayText = d.designationNo + " - " + d.designationNom
            }).ToList();

            ViewBag.ListOfDesignation = new SelectList(desigSelectList, "designationID", "displayText");

            // -------------------------------------------------------------------------------------------------PrestList

            List<tbl_sousFamille> libelleList = db.tbl_sousFamille.Where(f => f.isDeleted == false).ToList();
            var prestationSelectList = libelleList.Select(d => new
            {
                idSousFamille = d.idSousFamille,
                displayText = d.codeNom + " - " + d.nomSousFamille
            }).ToList();

            ViewBag.ListOfPrestation = new SelectList(prestationSelectList, "idSousFamille", "displayText");

            //--------------------------------------------------------------------------------------------------------------

            // Récupérer la liste des prestataires
            List<tbl_prestataire> PrestList = db.tbl_prestataire.ToList();

            // Sélectionner distinctement les valeurs de codePrestataire
            var distinctPrestataires = PrestList.Select(d => d.codePrestataire).Distinct().ToList();

            // Créer une liste d'objets anonymes contenant les valeurs idPrestataire et displayText (codePrestataire)
            var prestSelectList = distinctPrestataires.Select(code => new
            {
                idPrestataire = PrestList.FirstOrDefault(p => p.codePrestataire == code)?.idPrestataire, // Récupérer l'ID correspondant au codePrestataire
                displayText = code // Utiliser le codePrestataire comme texte d'affichage
            }).ToList();

            // Créer une SelectList à partir de la liste d'objets anonymes
            ViewBag.ListOfPrestataire = new SelectList(prestSelectList, "idPrestataire", "displayText");

            return View();
        }
        public JsonResult GetbsList(int page = 1, int pageSize = 5)
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

                var bsList = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.status == "validate")
                    .OrderBy(x => x.idBs) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new BulletinSoin
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,

                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,

                        codePrestataire = x.tbl_prestataire.codePrestataire,

                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = bsList.Count;

               

                return Json(bsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetbsList2(int page = 1, int pageSize = 5)
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

                var bsList = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.status == "validate")
                    .OrderBy(x => x.idBs) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(x => new BulletinSoin
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,

                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,

                        codePrestataire = x.tbl_prestataire.codePrestataire,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db.tbl_bulletinSoin.Count(x => x.isDeleted == false);

                return Json(new { data = bsList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetbsListNew(int page = 1, int pageSize = 5)
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

                var bsListNew = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.status == "new")
                    .OrderBy(x => x.idBs) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new BulletinSoinNew
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,

                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,

                        codePrestataire = x.tbl_prestataire.codePrestataire,

                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = bsListNew.Count;



                return Json(bsListNew, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }

        public JsonResult GetbsList2New(int page = 1, int pageSize = 5)
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

                var bsList = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.status == "new")
                    .OrderBy(x => x.idBs) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(x => new BulletinSoinNew
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,

                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,

                        codePrestataire = x.tbl_prestataire.codePrestataire,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db.tbl_bulletinSoin.Count(x => x.isDeleted == false);

                return Json(new { data = bsList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }
        // Méthode pour récupérer les données du bulletin de soin, de l'adhérent et de la rubrique associés à l'identifiant idBs
        public JsonResult GetbsById(int idBs)
        {
            try
            {
                var model = db.tbl_bulletinSoin
                    .Where(x => x.idBs == idBs && x.isDeleted == false)
                    .Select(x => new
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,
                        idAdherent = x.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,
                        matriculeCnam = x.tbl_adherent.matriculeCnam,
                        CIN_PASS = x.tbl_adherent.CIN_PASS,
                        adressAdh = x.tbl_adherent.adressAdh,
                        idPrestataire = x.idPrestataire,

                        Rubrique1 = x.tbl_rub1.Select(r => new
                        {
                            dateRub1 = r.dateRub1,
                            idSousFamille = r.idSousFamille,
                            honorairesRub1 = r.honorairesRub1,
                            deviseRub1 = r.deviseRub1,
                            matriculeFisacalRub1 = r.matriculeFisacalRub1
                        }).FirstOrDefault(),
                        Rubrique2 = x.tbl_rub2.Select(r => new
                        {
                            DE = r.DE,
                            DS = r.DS,
                            montantFrais = r.montantFrais,
                            deviseRub2 = r.deviseRub2,
                            matriculeFiscalRub2 = r.matriculeFiscalRub2
                        }).FirstOrDefault(),
                        Rubrique3 = x.tbl_rub3.Select(r => new
                        {
                            dateRub3 = r.dateRub3,
                            dents = r.dents,
                            actMedical = r.actMedical,
                            honorairesRub3 = r.honorairesRub3,
                            deviseRub3 = r.deviseRub3,
                            matriculeFiscalRub3 = r.matriculeFiscalRub3,
                            type = r.type
                        }).FirstOrDefault(),


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

        public JsonResult GetbsNewById(int idBs)
        {
            try
            {
                var model = db.tbl_bulletinSoin
                    .Where(x => x.idBs == idBs && x.isDeleted == false)
                    .Select(x => new
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,
                        idAdherent = x.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,
                        matriculeCnam = x.tbl_adherent.matriculeCnam,
                        CIN_PASS = x.tbl_adherent.CIN_PASS,
                        adressAdh = x.tbl_adherent.adressAdh,
                        idPrestataire = x.idPrestataire,
                        path = x.path, // Ajouter le chemin de la photo

                        Rubrique1 = x.tbl_rub1.Select(r => new
                        {
                            dateRub1 = r.dateRub1,
                            idSousFamille = r.idSousFamille,
                            honorairesRub1 = r.honorairesRub1,
                            deviseRub1 = r.deviseRub1,
                            matriculeFisacalRub1 = r.matriculeFisacalRub1
                        }).FirstOrDefault(),
                        Rubrique2 = x.tbl_rub2.Select(r => new
                        {
                            DE = r.DE,
                            DS = r.DS,
                            montantFrais = r.montantFrais,
                            deviseRub2 = r.deviseRub2,
                            matriculeFiscalRub2 = r.matriculeFiscalRub2
                        }).FirstOrDefault(),
                        Rubrique3 = x.tbl_rub3.Select(r => new
                        {
                            dateRub3 = r.dateRub3,
                            dents = r.dents,
                            actMedical = r.actMedical,
                            honorairesRub3 = r.honorairesRub3,
                            deviseRub3 = r.deviseRub3,
                            matriculeFiscalRub3 = r.matriculeFiscalRub3,
                            type = r.type
                        }).FirstOrDefault(),


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




        public JsonResult saveUpdatedData(BulletinSoin model, Adherent model2)
        {
            var result = false;
            var errors = "";

          
                    try
                    {
                        if (model.idBs > 0)
                        {
                            // Recherche du bulletin de soin existant
                            var bs = db.tbl_bulletinSoin.SingleOrDefault(x => x.isDeleted == false && x.idBs == model.idBs);
                          
                  
                    if (bs != null)
                            
                    {
                                // Mettre à jour les propriétés du bulletin de soin
                        bs.idPrestataire = model.idPrestataire;
                        bs.idAdherent = model.idAdherent;
                        bs.numBs = model.numBs;
                       

                        db.SaveChanges();
                                result = true;
                            }
                            else
                            {
                                errors = "Bulletin de soin non trouvé.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Gérer les exceptions ici
                        errors = "Une erreur s'est produite lors de la sauvegarde des données.";
                        // Log ex.Message si nécessaire
                    }
                
            
           
            return Json(new { success = result, errors = errors }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDataInDatabase(BulletinSoin model, Adherent model2, List<Rubrique1Model> Rubriques, List<Rubrique2Model> Rubriques2, List<Rubrique3Model> Rubriques3)
        {
            var result = false;

            if (model.idBs > 0)
            {
                // Recherche du bulletin de soin existant
                tbl_bulletinSoin bs = db.tbl_bulletinSoin.SingleOrDefault(x => x.isDeleted == false && x.idBs == model.idBs);

                // Recherche de l'adhérent associé
                tbl_adherent adh = db.tbl_adherent.SingleOrDefault(x => x.isDeleted == false && x.idAdherent == model.idAdherent);

                if (bs == null || adh == null)
                {
                    // Si le bulletin de soin ou l'adhérent associé n'existe pas, retourner un message d'erreur
                    return Json(new { success = false, message = "Bulletin de soin ou adhérent associé non trouvé." }, JsonRequestBehavior.AllowGet);
                }

                // Mettre à jour les propriétés du bulletin de soin
                bs.idAdherent = model.idAdherent;
                bs.numBs = model.numBs;

                bs.idPrestataire = model.idPrestataire;
                bs.path = model.path;
                // Mettre à jour les propriétés de l'adhérent
                adh.nomAdh = model.nomAdh;
                adh.prenomAdh = model.prenomAdh;
                adh.matriculeCnam = model.matriculeCnam;
                adh.CIN_PASS = model.CIN_PASS;
                adh.adressAdh = model.adressAdh;

                // Modification des rubriques associées
                if (Rubriques != null && Rubriques.Any())
                {
                    foreach (var sousRubrique1 in Rubriques)
                    {
                        tbl_rub1 rub1 = db.tbl_rub1.SingleOrDefault(r => r.idBs == model.idBs && r.idSousFamille == sousRubrique1.idSousFamille);
                        if (rub1 != null)
                        {
                            // Mettre à jour les propriétés de Rubrique1
                            rub1.dateRub1 = sousRubrique1.dateRub1;
                            rub1.idSousFamille = sousRubrique1.idSousFamille;
                            rub1.honorairesRub1 = sousRubrique1.honorairesRub1;
                            rub1.deviseRub1 = sousRubrique1.deviseRub1;
                            rub1.matriculeFisacalRub1 = sousRubrique1.matriculeFisacalRub1;
                        }
                        else
                        {
                            // Si la rubrique n'existe pas, la créer
                            rub1 = new tbl_rub1
                            {
                                idBs = model.idBs,
                                dateRub1 = sousRubrique1.dateRub1,
                                idSousFamille = sousRubrique1.idSousFamille,
                                honorairesRub1 = sousRubrique1.honorairesRub1,
                                deviseRub1 = sousRubrique1.deviseRub1,
                                matriculeFisacalRub1 = sousRubrique1.matriculeFisacalRub1
                            };
                            db.tbl_rub1.Add(rub1);
                        }
                    }
                }
                if (Rubriques2 != null && Rubriques2.Any())
                {
                    foreach (var sousRubrique2 in Rubriques2)
                    {
                        tbl_rub2 rub2 = db.tbl_rub2.SingleOrDefault(r => r.idBs == model.idBs);
                        if (rub2 != null)
                        {
                            // Mettre à jour les propriétés de Rubrique2
                            rub2.DE = sousRubrique2.DE;
                            rub2.DS = sousRubrique2.DS;
                            rub2.montantFrais = sousRubrique2.montantFrais;
                            rub2.deviseRub2 = sousRubrique2.deviseRub2;
                            rub2.matriculeFiscalRub2 = sousRubrique2.matriculeFiscalRub2;
                        }
                        else
                        {
                            // Si la rubrique n'existe pas, la créer
                            rub2 = new tbl_rub2
                            {
                                idBs = model.idBs,
                                DE = sousRubrique2.DE,
                                DS = sousRubrique2.DS,
                                montantFrais = sousRubrique2.montantFrais,
                                deviseRub2 = sousRubrique2.deviseRub2,
                                matriculeFiscalRub2 = sousRubrique2.matriculeFiscalRub2
                            };
                            db.tbl_rub2.Add(rub2);
                        }
                    }
                }

                if (Rubriques3 != null && Rubriques3.Any())
                {
                    foreach (var sousRubrique3 in Rubriques3)
                    {
                        tbl_rub3 rub3 = db.tbl_rub3.SingleOrDefault(r => r.idBs == model.idBs);
                        if (rub3 != null)
                        {
                            // Mettre à jour les propriétés de Rubrique3
                            rub3.dateRub3 = sousRubrique3.dateRub3;
                            rub3.dents = sousRubrique3.dents;
                            rub3.actMedical = sousRubrique3.actMedical;
                            rub3.honorairesRub3 = sousRubrique3.honorairesRub3;
                            rub3.deviseRub3 = sousRubrique3.deviseRub3;
                            rub3.matriculeFiscalRub3 = sousRubrique3.matriculeFiscalRub3;
                            rub3.type = sousRubrique3.type;
                        }
                        else
                        {
                            // Si la rubrique n'existe pas, la créer
                            rub3 = new tbl_rub3
                            {
                                idBs = model.idBs,
                                dateRub3 = sousRubrique3.dateRub3,
                                dents = sousRubrique3.dents,
                                actMedical = sousRubrique3.actMedical,
                                honorairesRub3 = sousRubrique3.honorairesRub3,
                                deviseRub3 = sousRubrique3.deviseRub3,
                                matriculeFiscalRub3 = sousRubrique3.matriculeFiscalRub3,
                                type = sousRubrique3.type
                            };
                            db.tbl_rub3.Add(rub3);
                        }
                    }
                }

                db.SaveChanges();
                result = true;
            }

            else
            {
                // Ajout d'un nouvel enregistrement
                tbl_bulletinSoin bs = new tbl_bulletinSoin();
                tbl_adherent adh = new tbl_adherent();

                // Affectez les propriétés de l'adhérent et du bulletin de soin
                bs.numBs = model.numBs;

                bs.idAdherent = model.idAdherent;
                adh.nomAdh = model.nomAdh;
                adh.prenomAdh = model.prenomAdh;
                adh.matriculeCnam = model.matriculeCnam;
                adh.CIN_PASS = model.CIN_PASS;
                adh.adressAdh = model.adressAdh;
                bs.idPrestataire = model.idPrestataire;
                bs.status = "validate";

                bs.isDeleted = false;
                db.tbl_bulletinSoin.Add(bs);

                // Ajoutez les rubriques uniquement si elles ont des données
                if (Rubriques != null && Rubriques.Any())
                {
                    foreach (var rubrique in Rubriques)
                    {
                        if (rubrique != null)
                        {
                            tbl_rub1 newRubrique = new tbl_rub1
                            {
                                idBs = model.idBs,
                                dateRub1 = rubrique.dateRub1,
                                idSousFamille = rubrique.idSousFamille,
                                honorairesRub1 = rubrique.honorairesRub1,
                                deviseRub1 = rubrique.deviseRub1,
                                montant_change = rubrique.montant_change,
                                montant_estimé=rubrique.montant_estimé,
                                matriculeFisacalRub1 = rubrique.matriculeFisacalRub1


                            };
                            db.tbl_rub1.Add(newRubrique);
                        }
                    }
                }

                // Ajoutez les rubriques Rubrique2 uniquement si elles ont des données
                if (Rubriques2 != null && Rubriques2.Any())
                {
                    foreach (var rubrique2 in Rubriques2)
                    {
                        if (rubrique2 != null)
                        {
                            tbl_rub2 newRubrique2 = new tbl_rub2
                            {
                                idBs = model.idBs,
                                DE = rubrique2.DE,
                                DS = rubrique2.DS,
                                montantFrais = rubrique2.montantFrais,
                                deviseRub2 = rubrique2.deviseRub2,
                                montant_change2 = rubrique2.montant_change2,

                                matriculeFiscalRub2 = rubrique2.matriculeFiscalRub2
                            };
                            db.tbl_rub2.Add(newRubrique2);
                        }

                    }
                }

                // Ajoutez les rubriques Rubrique3 uniquement si elles ont des données
                if (Rubriques3 != null && Rubriques3.Any())
                {
                    foreach (var rubrique3 in Rubriques3)
                    {
                        if (rubrique3 != null)
                        {
                            tbl_rub3 newRubrique3 = new tbl_rub3
                            {
                                idBs = model.idBs,
                                dateRub3 = rubrique3.dateRub3,
                                dents = rubrique3.dents,
                                actMedical = rubrique3.actMedical,
                                honorairesRub3 = rubrique3.honorairesRub3,
                                deviseRub3 = rubrique3.deviseRub3,
                                montant_change3 = rubrique3.montant_change3,

                                matriculeFiscalRub3 = rubrique3.matriculeFiscalRub3,
                                type = rubrique3.type
                            };
                            db.tbl_rub3.Add(newRubrique3);
                        }
                    }
                }

                db.SaveChanges();
                result = true;
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeletebsRecord(int idBs)
        {
            bool result = false;
            tbl_bulletinSoin bs = db.tbl_bulletinSoin.SingleOrDefault(x => x.isDeleted == false && x.idBs == idBs);
            if (bs != null)
            {
                bs.isDeleted = true;
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FilterBsById(int numBs)
        {
            try
            {
                var filteredBsList = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.numBs == numBs && x.status == "validate")
                    .Select(x => new BulletinSoin
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,
                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,
                       
                        codePrestataire = x.tbl_prestataire.codePrestataire,
                    })
                    .ToList();

                return Json(filteredBsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs ici
                return Json(new { success = false, message = "Une erreur s'est produite lors de la recherche des bulletins de soins par ID." });
            }
        }

        public ActionResult FilterBsByAdherentId(int idAdherent)
        {
            try
            {
                var filteredBsList = db.tbl_bulletinSoin
                    .Where(x => x.isDeleted == false && x.idAdherent == idAdherent && x.status == "validate")
                    .Select(x => new BulletinSoin
                    {
                        idBs = x.idBs,
                        numBs = x.numBs,
                        idAdherent = x.tbl_adherent.idAdherent,
                        nomAdh = x.tbl_adherent.nomAdh,
                        prenomAdh = x.tbl_adherent.prenomAdh,
                      
                        codePrestataire = x.tbl_prestataire.codePrestataire,

                    }).ToList();

                return Json(filteredBsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs ici
                return Json(new { error = ex.Message });
            }
        }

        // Méthode pour récupérer les rubriques associées à un bulletin de soin
        public JsonResult GetRubriquesByIdBs(int idBs)
        {
            try
            {
                var rubriques = db.tbl_rub1
                    .Where(r => r.idBs == idBs)
                    .Select(r => new RubriqueViewModel
                    {
                        idRub1 = r.idRub1,
                        dateRub1 = (DateTime)r.dateRub1,
                        codeNom = r.tbl_sousFamille.codeNom,
                        nomSousFamille = r.tbl_sousFamille.nomSousFamille,
                        honorairesRub1 = (decimal)r.honorairesRub1,
                        deviseRub1 = r.deviseRub1,
                        montant_change = r.montant_change,
                        montant_estimé = r.montant_estimé,

                        matriculeFisacalRub1 = r.matriculeFisacalRub1,
                        // Ajoutez d'autres propriétés ici
                    })
                    .ToList();



                return Json(rubriques, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Une erreur s'est produite lors de la récupération des rubriques." });
            }
        }

        public JsonResult GetRubriques2ByIdBs(int idBs)
        {
            try
            {
                var rubriques2 = db.tbl_rub2
                    .Where(r => r.idBs == idBs)
                    .Select(r => new Rubrique2Model
                    {
                        idRub2 = r.idRub2,
                        DE = r.DE,
                        DS = r.DS,
                        montantFrais = r.montantFrais,
                        deviseRub2 = r.deviseRub2,
                        montant_change2 = r.montant_change2,

                        matriculeFiscalRub2 = r.matriculeFiscalRub2
                        // Ajoutez d'autres propriétés ici
                    })
                    .ToList();

                return Json(rubriques2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Une erreur s'est produite lors de la récupération des rubriques." });
            }
        }

        public JsonResult GetRubriques3ByIdBs(int idBs)
        {
            try
            {
                var rubriques3 = db.tbl_rub3
                    .Where(r => r.idBs == idBs)
                    .Select(r => new RubriqueViewModel
                    {
                        idRub3 = r.idRub3,
                        dateRub3 = r.dateRub3,
                        dents = r.dents,
                        actMedical = r.actMedical,
                        honorairesRub3 = r.honorairesRub3,
                        deviseRub3 = r.deviseRub3,
                        montant_change3 = r.montant_change3,

                        matriculeFiscalRub3 = r.matriculeFiscalRub3,
                        type = r.type
                    })
                    .ToList();

                return Json(rubriques3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Une erreur s'est produite lors de la récupération des rubriques." });
            }
        }

        [HttpPost]
        public ActionResult SynchroniserRubriques(Rubrique1Model[] rubriquesData)
        {
            try
            {
                if (rubriquesData != null && rubriquesData.Length > 0)
                {
                    foreach (var rubriqueData in rubriquesData)
                    {
                        // Créez une nouvelle instance de votre modèle Rubrique1Model
                        var nouvelleRubrique = new tbl_rub1
                        {
                            dateRub1 = rubriqueData.dateRub1,
                            idSousFamille = rubriqueData.idSousFamille,
                            honorairesRub1 = rubriqueData.honorairesRub1,
                            deviseRub1 = rubriqueData.deviseRub1,
                            montant_change = rubriqueData.montant_change,
                            montant_estimé = rubriqueData.montant_estimé,

                            matriculeFisacalRub1 = rubriqueData.matriculeFisacalRub1,
                            idBs = rubriqueData.idBs
                        };

                        // Ajoutez la nouvelle rubrique à votre contexte de base de données
                        db.tbl_rub1.Add(nouvelleRubrique);
                    }

                    // Enregistrez les modifications dans la base de données
                    db.SaveChanges();

                    return Json(new { success = true, message = "Données synchronisées avec succès." });
                }

                else
                {
                    return Json(new { success = false, message = "Aucune donnée reçue du client." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la synchronisation des données." });
            }
        }

        [HttpPost]
        public ActionResult SynchroniserRubriques3(Rubrique3Model[] rubriquesData3)
        {
            try
            {
                if (rubriquesData3 != null && rubriquesData3.Length > 0)
                {
                    foreach (var rubriqueData3 in rubriquesData3)
                    {
                        // Créez une nouvelle instance de votre modèle Rubrique1Model
                        var nouvelleRubrique3 = new tbl_rub3
                        {
                            dateRub3 = rubriqueData3.dateRub3,
                            dents = rubriqueData3.dents,
                            actMedical = rubriqueData3.actMedical,
                            honorairesRub3 = rubriqueData3.honorairesRub3,
                            deviseRub3 = rubriqueData3.deviseRub3,
                            montant_change3 = rubriqueData3.montant_change3,

                            matriculeFiscalRub3 = rubriqueData3.matriculeFiscalRub3,
                            type = rubriqueData3.type,
                            idBs = rubriqueData3.idBs
                        };

                        // Ajoutez la nouvelle rubrique à votre contexte de base de données
                        db.tbl_rub3.Add(nouvelleRubrique3);
                    }

                    // Enregistrez les modifications dans la base de données
                    db.SaveChanges();

                    return Json(new { success = true, message = "Données synchronisées avec succès." });
                }

                else
                {
                    return Json(new { success = false, message = "Aucune donnée reçue du client." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la synchronisation des données." });
            }
        }


        [HttpPost]
        public ActionResult SynchroniserRubriques2(Rubrique2Model[] rubriquesData2)
        {
            try
            {
                if (rubriquesData2 != null && rubriquesData2.Length > 0)
                {
                    foreach (var rubriqueData2 in rubriquesData2)
                    {
                        // Créez une nouvelle instance de votre modèle Rubrique2Model
                        var nouvelleRubrique2 = new tbl_rub2
                        {
                            DE = rubriqueData2.DE,
                            DS = rubriqueData2.DS,
                            montantFrais = rubriqueData2.montantFrais,
                            deviseRub2 = rubriqueData2.deviseRub2,
                            montant_change2 = rubriqueData2.montant_change2,

                            matriculeFiscalRub2 = rubriqueData2.matriculeFiscalRub2,
                            idBs = rubriqueData2.idBs
                        };

                        // Ajoutez la nouvelle rubrique à votre contexte de base de données
                        db.tbl_rub2.Add(nouvelleRubrique2);
                    }

                    // Enregistrez les modifications dans la base de données
                    db.SaveChanges();

                    return Json(new { success = true, message = "Données synchronisées avec succès." });
                }
                else
                {
                    return Json(new { success = false, message = "Aucune donnée reçue du client." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la synchronisation des données." });
            }
        }


        [HttpPost]
        public ActionResult DeleteRubrique(int idRubrique)
        {
            try
            {

                // Trouver la rubrique dans la base de données
                var rubriqueToDelete = db.tbl_rub1.Find(idRubrique);
                if (rubriqueToDelete != null)
                {
                    // Supprimer la rubrique de la base de données
                    db.tbl_rub1.Remove(rubriqueToDelete);
                    db.SaveChanges();

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
        public ActionResult DeleteRubrique2(int idRubrique2)
        {
            try
            {

                // Trouver la rubrique dans la base de données
                var rubriqueToDelete = db.tbl_rub2.Find(idRubrique2);
                if (rubriqueToDelete != null)
                {
                    // Supprimer la rubrique de la base de données
                    db.tbl_rub2.Remove(rubriqueToDelete);
                    db.SaveChanges();

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

        public ActionResult DeleteRubrique3(int idRubrique3)
        {
            try
            {

                // Trouver la rubrique dans la base de données
                var rubriqueToDelete = db.tbl_rub3.Find(idRubrique3);
                if (rubriqueToDelete != null)
                {
                    // Supprimer la rubrique de la base de données
                    db.tbl_rub3.Remove(rubriqueToDelete);
                    db.SaveChanges();

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


        [HttpGet]
        public JsonResult GetAdherentDetails(int idAdherent)
        {
            try
            {
                var model = db.tbl_adherent
                    .Where(a => a.idAdherent == idAdherent && a.isDeleted == false)
                    .Select(a => new
                    {
                        idAdherent = a.idAdherent,
                        nomAdh = a.nomAdh,
                        prenomAdh = a.prenomAdh,
                        matriculeCnam = a.matriculeCnam,
                        CIN_PASS = a.CIN_PASS,
                        adressAdh = a.adressAdh
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


        [HttpGet]
        public JsonResult GetTauxDeChange(string selectedCurrency, DateTime rubriqueDate, Rubrique1Model rubriqueModel, tauxChange tauxModel)
        {
            if (selectedCurrency == "TND")
            {
                // Si la devise est TND, retourner les honoraires sans conversion
                return Json(new { taux = 1, montantChange = rubriqueModel.honorairesRub1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Recherche du taux de change correspondant dans la base de données
                var taux = db.tbl_taux_de_change
                    .FirstOrDefault(x => x.devise_locale == selectedCurrency &&
                                         x.date_debut <= rubriqueDate &&
                                         x.date_fin >= rubriqueDate &&
                                         !x.is_deleted);

                if (taux != null)
                {
                    // Effectuez vos opérations de conversion en fonction du modèle Rubrique1Model et tauxChange
                    double montantChange = 0.0;
                    double montantEstime=0.0;

                    // Vérifiez si honorairesRub1 a une valeur
                    if (rubriqueModel.honorairesRub1.HasValue)
                    {
                        // Calculez le montant converti uniquement si honorairesRub1 a une valeur
                        montantChange = taux.taux * (double)rubriqueModel.honorairesRub1.Value;
                    }

                    // Retournez le résultat sous forme de JSON avec le taux de change et le montant converti
                    return Json(new { taux = taux.taux, montantChange = montantChange }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Si aucun taux de change n'est trouvé, retournez une erreur
                    return Json(new { error = "Aucun taux de change trouvé pour la devise sélectionnée et la date de la rubrique." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public JsonResult GetTauxDeChange2(string selectedCurrency, DateTime rubriqueDate, Rubrique2Model rubriqueModel, tauxChange tauxModel)
        {
            if (selectedCurrency == "TND")
            {
                // Si la devise est TND, retourner les honoraires sans conversion
                return Json(new { taux = 1, montantChange = rubriqueModel.montantFrais }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                // Recherche du taux de change correspondant dans la base de données
                var taux = db.tbl_taux_de_change
                .FirstOrDefault(x => x.devise_locale == selectedCurrency &&
                                     x.date_debut <= rubriqueDate &&
                                     x.date_fin >= rubriqueDate &&
                                     !x.is_deleted);

                if (taux != null)
                {
                    // Effectuez vos opérations de conversion en fonction du modèle Rubrique2Model et tauxChange
                    double montantChange = 0.0;

                    // Vérifiez si montantFrais a une valeur
                    if (rubriqueModel.montantFrais.HasValue)
                    {
                        // Calculez le montant converti uniquement si montantFrais a une valeur
                        montantChange = taux.taux * (double)rubriqueModel.montantFrais.Value;
                    }

                    // Retournez le résultat sous forme de JSON avec le taux de change et le montant converti
                    return Json(new { taux = taux.taux, montantChange = montantChange }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Si aucun taux de change n'est trouvé, retournez une erreur
                    return Json(new { error = "Aucun taux de change trouvé pour la devise sélectionnée et la date de la rubrique." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public JsonResult GetTauxDeChange3(string selectedCurrency, DateTime rubriqueDate, Rubrique3Model rubriqueModel, tauxChange tauxModel)
        {
            if (selectedCurrency == "TND")
            {
                // Si la devise est TND, retourner les honoraires sans conversion
                return Json(new { taux = 1, montantChange = rubriqueModel.honorairesRub3 }, JsonRequestBehavior.AllowGet);
            }
            else
            {


                // Recherche du taux de change correspondant dans la base de données
                var taux = db.tbl_taux_de_change
                .FirstOrDefault(x => x.devise_locale == selectedCurrency &&
                                     x.date_debut <= rubriqueDate &&
                                     x.date_fin >= rubriqueDate &&
                                     !x.is_deleted);

                if (taux != null)
                {
                    // Effectuez vos opérations de conversion en fonction du modèle Rubrique3Model et tauxChange
                    double montantChange = 0.0;

                    // Vérifiez si honorairesRub3 a une valeur
                    if (rubriqueModel.honorairesRub3.HasValue)
                    {
                        // Calculez le montant converti uniquement si honorairesRub3 a une valeur
                        montantChange = taux.taux * (double)rubriqueModel.honorairesRub3.Value;
                    }

                    // Retournez le résultat sous forme de JSON avec le taux de change et le montant converti
                    return Json(new { taux = taux.taux, montantChange = montantChange }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Si aucun taux de change n'est trouvé, retournez une erreur
                    return Json(new { error = "Aucun taux de change trouvé pour la devise sélectionnée et la date de la rubrique." }, JsonRequestBehavior.AllowGet);
                }
            }

        }

      
        [HttpGet]
        public ActionResult GetValeurByIdSousFamille(int idSousFamille)
        {
            var sf = db.tbl_sousFamille
                .Where(s => s.idSousFamille == idSousFamille && s.isDeleted == false)
                .Select(s => new
                {
                    Valeur = s.valeur,
                    Unite = s.unité,
                    Max = s.max// Assurez-vous de remplacer "unite" par le nom correct de la colonne dans votre base de données
                })
                .FirstOrDefault();

            return Json(sf, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult ValiderBulletin(int idBs)
        {
            try
            {
                var bulletin = db.tbl_bulletinSoin.FirstOrDefault(x => x.idBs == idBs);
                if (bulletin != null)
                {
                    bulletin.status = "validate";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { error = "Bulletin de soin non trouvé." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "Une erreur s'est produite lors de la validation du bulletin de soin." });
            }
        }

        [HttpPost]
        public JsonResult RejeterBulletin(int idBs)
        {
            try
            {
                var bulletin = db.tbl_bulletinSoin.FirstOrDefault(x => x.idBs == idBs);
                if (bulletin != null)
                {
                    bulletin.isDeleted = true;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { error = "Bulletin de soin non trouvé." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "Une erreur s'est produite lors du rejet du bulletin de soin." });
            }
        }






    }
}



