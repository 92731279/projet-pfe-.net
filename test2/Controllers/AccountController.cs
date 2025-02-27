using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace work.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        void connectionString()
        {
            con.ConnectionString = "data source = DESKTOP-84O94IB\\SQLEXPRESS; database = BsDB; integrated security = SSPI;";
        }

        // Méthode pour récupérer la liste des ID d'adhérents depuis la base de données
        private List<int> GetAdherentIds()
        {
            List<int> adherentIds = new List<int>();

            try
            {
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT idAdherent FROM tbl_adherent WHERE isDeleted = 0";
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    adherentIds.Add(Convert.ToInt32(dr["idAdherent"]));
                }
            }
            finally
            {
                con.Close();
            }

            return adherentIds;
        }

        [HttpPost]
        public ActionResult Verify(Account acc, Adherent adh)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * FROM tbl_login WHERE username = @username AND paassword = @password";

            com.Parameters.AddWithValue("@username", acc.Name);
            com.Parameters.AddWithValue("@password", acc.password);

            dr = com.ExecuteReader();

            // Si une ligne correspondante est trouvée
            if (dr.Read())
            {
                // Récupérer le rôle de l'utilisateur
                string role = dr["role"].ToString();

                con.Close();

                // Vérifier le rôle de l'utilisateur
                if (role == "Admin")
                {
                    // Rediriger vers la vue "Index"
                    return View("Index");
                }
                else if (role == "Admin Système")
                {
                    return RedirectToAction("HomePage", "User");
                }
                else if (role == "Adhérent")
                {

               
                    // Vérifier si le nom d'utilisateur correspond à l'ID de l'adhérent
                    List<int> adherentIds = GetAdherentIds();
                    if (adherentIds.Contains(Convert.ToInt32(acc.Name)))
                    {
                        return RedirectToAction("OpenSession", new { username = acc.Name });
                    }
                    else
                    {
                        ModelState.AddModelError("", "L'identifiant de l'adhérent ne correspond pas.");
                        return View("Login");
                        // Redirection ou traitement supplémentaire si nécessaire
                        // Par exemple, ouverture d'une session pour l'adhérent
                    }
                }
                else if (role == "Gestionnaire BS")
                {
                    return View("IndexA1");
                }
                else if (role == "Gestionnaire Adhérent")
                {
                    return View("IndexA2");
                }
                else if (role == "Gestionnaire TR")
                {
                    return View("IndexA3");
                }
                else
                {
                    ModelState.AddModelError("", "La connexion a échoué. Veuillez vérifier votre nom d'utilisateur et votre mot de passe.");
                    return View("Login");
                }
            }
            else
            {
                con.Close();
                ModelState.AddModelError("", "La connexion a échoué. Veuillez vérifier votre nom d'utilisateur et votre mot de passe.");
                return View("Login");
            }
        }

        public ActionResult Index()
        {
            return View("Index");
        }


        [HttpPost]
        public ActionResult VerifyUsername(string username)
        {
            // Recherche de l'adhérent correspondant au nom d'utilisateur saisi
            using (var db = new BsDBEntities25())
            {
                var adherent = db.tbl_adherent.FirstOrDefault(a => (a.nomAdh + " " + a.prenomAdh) == username);

                if (adherent != null)
                {
                    // Ouverture d'une session pour l'adhérent trouvé
                    Session["AdherentId"] = adherent.idAdherent;
                    return Json(new { success = true, nomAdh = adherent.nomAdh }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Aucun adhérent trouvé avec le nom d'utilisateur saisi
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }



        }

        public ActionResult OpenSession(string username)
        {
            // Stockez le nom d'utilisateur dans la session
            Session["username"] = username;
            // Redirigez l'utilisateur vers la page de tableau de bord ou une autre page après la connexion
            return RedirectToAction("Dashboard2");
        }

        public ActionResult Dashboard()
        {
            // Vérifier si le nom d'utilisateur est présent dans la session
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();
                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities27())
                {
                    var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                    if (adherent != null)
                    {
                        // Rechercher les bulletins de soin associés à cet adhérent
                        var bulletins = db.tbl_bulletinSoin.Where(b => b.idAdherent == adherent.idAdherent && b.isDeleted == false && b.status == "validate").ToList();

                        // Convertir les bulletins de type tbl_bulletinSoin en type BulletinSoin
                        List<BulletinSoin> listeBulletins = bulletins.Select(b => new BulletinSoin
                        {
                            idBs = b.idBs,
                            numBs = b.numBs,

                            idAdherent = b.idAdherent,
                            idPrestataire = b.idPrestataire
                            // Assurez-vous de mapper toutes les propriétés nécessaires
                        }).ToList();

                        // Créer une instance du modèle contenant à la fois l'adhérent et ses bulletins de soin
                        var viewModel = new DashboardViewModel
                        {
                            Adherent = adherent,
                            BulletinSoin = listeBulletins
                        };

                        // Afficher la vue correspondante pour l'adhérent connecté avec les bulletins de soin
                        return View("Dashboard", viewModel);
                    }
                }
            }
            // Rediriger vers la page de connexion si l'adhérent n'est pas connecté
            return RedirectToAction("Login");
        }


        // Action pour la déconnexion
        [HttpPost]
        public ActionResult Logout()
        {
            // Effacer les données de session pour déconnecter l'utilisateur
            Session.Clear();

            // Rediriger vers la page de connexion
            return RedirectToAction("Login");
        }


        public ActionResult Dashboard2()
        {
            // Vérifier si le nom d'utilisateur est présent dans la session
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();

                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities31())
                {

                    //--------------------------------------------------------------------------------------------------
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
                    //-------------------------------------------------------------------------------------------------------------------------------
                    List<tbl_sousFamille> libelleList = db.tbl_sousFamille.Where(f => f.isDeleted == false).ToList();
                    var prestationSelectList = libelleList.Select(d => new
                    {
                        idSousFamille = d.idSousFamille,
                        displayText = d.codeNom + " - " + d.nomSousFamille
                    }).ToList();

                    ViewBag.ListOfPrestation = new SelectList(prestationSelectList, "idSousFamille", "displayText");
                    //------------------------------------------------------------------------------------------------------------------------------------------

                    var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                    if (adherent != null)
                    {
                        // Rechercher les bulletins de soin associés à cet adhérent
                        var bulletins = db.tbl_bulletinSoin
                       .Where(b => b.idAdherent == adherent.idAdherent && b.isDeleted == false && b.status == "validate")
                            .ToList();

                        // Convertir les bulletins de type tbl_bulletinSoin en type BulletinSoin
                        List<BulletinSoin> listeBulletins = new List<BulletinSoin>();
                        foreach (var bulletin in bulletins)
                        {
                            var bulletinSoin = new BulletinSoin
                            {
                                idBs = bulletin.idBs,
                                numBs = bulletin.numBs,
                                idAdherent = bulletin.idAdherent,
                                codePrestataire = bulletin.tbl_prestataire.codePrestataire,
                                // Assurez-vous de mapper toutes les propriétés nécessaires
                                status = bulletin.status

                            };


                            var rubrique1Infos = db.tbl_rub1
                                .Where(r => r.idBs == bulletin.idBs && r != null)
                                .ToList();
                            var rubrique2Infos = db.tbl_rub2
                                  .Where(r => r.idBs == bulletin.idBs && r != null)
                                  .ToList();
                            var rubrique3Infos = db.tbl_rub3
                                 .Where(r => r.idBs == bulletin.idBs && r != null)
                                 .ToList();
                            if (rubrique1Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriquesAssociées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo in rubrique1Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        dateRub1 = rubriqueInfo.dateRub1.HasValue ? (DateTime)rubriqueInfo.dateRub1 : DateTime.MinValue,
                                        idSousFamille = rubriqueInfo.idSousFamille.HasValue ? (int)rubriqueInfo.idSousFamille : 0,
                                        codeNom = rubriqueInfo.tbl_sousFamille != null ? rubriqueInfo.tbl_sousFamille.codeNom : null,
                                        nomSousFamille = rubriqueInfo.tbl_sousFamille != null ? rubriqueInfo.tbl_sousFamille.nomSousFamille : null,
                                        honorairesRub1 = rubriqueInfo.honorairesRub1.HasValue ? (decimal)rubriqueInfo.honorairesRub1 : 0,
                                        deviseRub1 = rubriqueInfo.deviseRub1,
                                        matriculeFisacalRub1 = rubriqueInfo.matriculeFisacalRub1,
                                        // Ajoutez d'autres propriétés de la rubrique 1 au besoin
                                    };

                                    rubriquesAssociées.Add(rubriqueViewModel);
                                }

                                bulletinSoin.Rubrique = rubriquesAssociées;
                            }

                            else
                            {
                                // Si aucune information de la rubrique 1 n'est trouvée, attribuer null à bulletinSoin.Rubrique
                                bulletinSoin.Rubrique = null;
                            }
                            if (rubrique2Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriques2Associées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo2 in rubrique2Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        DE = rubriqueInfo2.DE.HasValue ? (DateTime)rubriqueInfo2.DE : DateTime.MinValue,
                                        DS = rubriqueInfo2.DS.HasValue ? (DateTime)rubriqueInfo2.DS : DateTime.MinValue,
                                        montantFrais = rubriqueInfo2.montantFrais.HasValue ? (decimal)rubriqueInfo2.montantFrais : 0,
                                        deviseRub2 = rubriqueInfo2.deviseRub2,
                                        matriculeFiscalRub2 = rubriqueInfo2.matriculeFiscalRub2,
                                        montant_change2 = rubriqueInfo2.montant_change2.HasValue ? (double)rubriqueInfo2.montant_change2 : 0

                                        // Ajoutez d'autres propriétés de la rubrique 2 au besoin
                                    };

                                    rubriques2Associées.Add(rubriqueViewModel);
                                }

                                // Ajouter les rubriques 2 associées à la liste de rubriques du bulletin
                                bulletinSoin.Rubrique_2 = rubriques2Associées;
                            }
                            else
                            {
                                // Si aucune information de la rubrique 1 n'est trouvée, attribuer null à bulletinSoin.Rubrique
                                bulletinSoin.Rubrique_2 = null;
                            }

                            if (rubrique3Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriques3Associées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo3 in rubrique3Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        idRub3 = rubriqueInfo3.idRub3,
                                        dateRub3 = rubriqueInfo3.dateRub3.HasValue ? (DateTime)rubriqueInfo3.dateRub3 : DateTime.MinValue,
                                        dents = rubriqueInfo3.dents,
                                        actMedical = rubriqueInfo3.actMedical,
                                        honorairesRub3 = rubriqueInfo3.honorairesRub3.HasValue ? (decimal)rubriqueInfo3.honorairesRub3 : 0,
                                        deviseRub3 = rubriqueInfo3.deviseRub3,
                                        matriculeFiscalRub3 = rubriqueInfo3.matriculeFiscalRub3,
                                        type = rubriqueInfo3.type,
                                        idBs = rubriqueInfo3.idBs,
                                        montant_change3 = rubriqueInfo3.montant_change3.HasValue ? (double)rubriqueInfo3.montant_change3 : 0
                                        // Ajoutez d'autres propriétés de la rubrique 3 au besoin
                                    };

                                    rubriques3Associées.Add(rubriqueViewModel);
                                }

                                // Ajouter les rubriques 3 associées à la liste de rubriques du bulletin
                                bulletinSoin.Rubrique_3 = rubriques3Associées;
                            }
                            else
                            {
                                // Si aucune information de la rubrique 3 n'est trouvée, attribuer null à bulletinSoin.Rubrique_3
                                bulletinSoin.Rubrique_3 = null;
                            }


                            // Ajouter bulletinSoin à la liste
                            listeBulletins.Add(bulletinSoin);
                        }


                        var bulletinsNew = db.tbl_bulletinSoin
                        .Where(b => b.idAdherent == adherent.idAdherent && b.isDeleted == false && b.status == "new")
                        .ToList();

                        // Convertir les bulletins de type tbl_bulletinSoin en type BulletinSoin pour le statut "new"
                        List<BulletinSoin> listeBulletinsNew = new List<BulletinSoin>();
                        foreach (var bulletinNew in bulletinsNew)
                        {
                            var bulletinSoinNew = new BulletinSoin
                            {
                                idBs = bulletinNew.idBs,
                                numBs = bulletinNew.numBs,
                                path = bulletinNew.path,
                                idAdherent = bulletinNew.idAdherent,
                                codePrestataire = bulletinNew.tbl_prestataire.codePrestataire,
                                status = bulletinNew.status
                                // Assurez-vous de mapper toutes les propriétés nécessaires
                            };

                            // Récupérer les informations des rubriques associées au bulletin de soin pour le statut "new"
                            // Vous pouvez ajouter la récupération des informations des rubriques ici pour le statut "new"
                            var rubrique1Infos = db.tbl_rub1
                                .Where(r => r.idBs == bulletinNew.idBs && r != null)
                                .ToList();
                            var rubrique2Infos = db.tbl_rub2
                                  .Where(r => r.idBs == bulletinNew.idBs && r != null)
                                  .ToList();
                            var rubrique3Infos = db.tbl_rub3
                                 .Where(r => r.idBs == bulletinNew.idBs && r != null)
                                 .ToList();
                            if (rubrique1Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriquesAssociées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo in rubrique1Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        dateRub1 = rubriqueInfo.dateRub1.HasValue ? (DateTime)rubriqueInfo.dateRub1 : DateTime.MinValue,
                                        idSousFamille = rubriqueInfo.idSousFamille.HasValue ? (int)rubriqueInfo.idSousFamille : 0,
                                        codeNom = rubriqueInfo.tbl_sousFamille != null ? rubriqueInfo.tbl_sousFamille.codeNom : null,
                                        nomSousFamille = rubriqueInfo.tbl_sousFamille != null ? rubriqueInfo.tbl_sousFamille.nomSousFamille : null,
                                        honorairesRub1 = rubriqueInfo.honorairesRub1.HasValue ? (decimal)rubriqueInfo.honorairesRub1 : 0,
                                        deviseRub1 = rubriqueInfo.deviseRub1,
                                        matriculeFisacalRub1 = rubriqueInfo.matriculeFisacalRub1,
                                        // Ajoutez d'autres propriétés de la rubrique 1 au besoin
                                    };

                                    rubriquesAssociées.Add(rubriqueViewModel);
                                }

                                bulletinSoinNew.Rubrique = rubriquesAssociées;
                            }

                            else
                            {
                                // Si aucune information de la rubrique 1 n'est trouvée, attribuer null à bulletinSoin.Rubrique
                                bulletinSoinNew.Rubrique = null;
                            }
                            if (rubrique2Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriques2Associées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo2 in rubrique2Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        DE = rubriqueInfo2.DE.HasValue ? (DateTime)rubriqueInfo2.DE : DateTime.MinValue,
                                        DS = rubriqueInfo2.DS.HasValue ? (DateTime)rubriqueInfo2.DS : DateTime.MinValue,
                                        montantFrais = rubriqueInfo2.montantFrais.HasValue ? (decimal)rubriqueInfo2.montantFrais : 0,
                                        deviseRub2 = rubriqueInfo2.deviseRub2,
                                        matriculeFiscalRub2 = rubriqueInfo2.matriculeFiscalRub2,
                                        montant_change2 = rubriqueInfo2.montant_change2.HasValue ? (double)rubriqueInfo2.montant_change2 : 0

                                        // Ajoutez d'autres propriétés de la rubrique 2 au besoin
                                    };

                                    rubriques2Associées.Add(rubriqueViewModel);
                                }

                                // Ajouter les rubriques 2 associées à la liste de rubriques du bulletin
                                bulletinSoinNew.Rubrique_2 = rubriques2Associées;
                            }
                            else
                            {
                                // Si aucune information de la rubrique 1 n'est trouvée, attribuer null à bulletinSoin.Rubrique
                                bulletinSoinNew.Rubrique_2 = null;
                            }

                            if (rubrique3Infos.Any())
                            {
                                // Liste pour stocker les RubriqueViewModel associées au même idBs
                                var rubriques3Associées = new List<RubriqueViewModel>();

                                // Boucle pour ajouter chaque RubriqueViewModel à la liste
                                foreach (var rubriqueInfo3 in rubrique3Infos)
                                {
                                    var rubriqueViewModel = new RubriqueViewModel
                                    {
                                        idRub3 = rubriqueInfo3.idRub3,
                                        dateRub3 = rubriqueInfo3.dateRub3.HasValue ? (DateTime)rubriqueInfo3.dateRub3 : DateTime.MinValue,
                                        dents = rubriqueInfo3.dents,
                                        actMedical = rubriqueInfo3.actMedical,
                                        honorairesRub3 = rubriqueInfo3.honorairesRub3.HasValue ? (decimal)rubriqueInfo3.honorairesRub3 : 0,
                                        deviseRub3 = rubriqueInfo3.deviseRub3,
                                        matriculeFiscalRub3 = rubriqueInfo3.matriculeFiscalRub3,
                                        type = rubriqueInfo3.type,
                                        idBs = rubriqueInfo3.idBs,
                                        montant_change3 = rubriqueInfo3.montant_change3.HasValue ? (double)rubriqueInfo3.montant_change3 : 0
                                        // Ajoutez d'autres propriétés de la rubrique 3 au besoin
                                    };

                                    rubriques3Associées.Add(rubriqueViewModel);
                                }

                                // Ajouter les rubriques 3 associées à la liste de rubriques du bulletin
                                bulletinSoinNew.Rubrique_3 = rubriques3Associées;
                            }
                            else
                            {
                                // Si aucune information de la rubrique 3 n'est trouvée, attribuer null à bulletinSoin.Rubrique_3
                                bulletinSoinNew.Rubrique_3 = null;
                            }

                            // Ajouter bulletinSoinNew à la liste des bulletins avec le statut "new"
                            listeBulletinsNew.Add(bulletinSoinNew);
                        }


                        // Créer une instance du modèle contenant à la fois l'adhérent et ses bulletins de soin
                        var viewModel = new DashboardViewModel
                        {
                            Adherent = adherent,
                            BulletinSoin = listeBulletins,
                            BulletinSoinNew = listeBulletinsNew

                        };

                        // Afficher la vue correspondante pour l'adhérent connecté avec les bulletins de soin
                        return View(viewModel); // Pas besoin de spécifier le nom de la vue ici
                    }
                }
            }
            // Rediriger vers la page de connexion si l'adhérent n'est pas connecté
            return RedirectToAction("Login");
        }

        public ActionResult adhInfo2()
        {
            if (Session["username"] != null)
            {

                string username = Session["username"].ToString();

                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities25())
                {
                    var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                    if (adherent != null)
                    {
                        // Rechercher les bulletins de soin associés à cet adhérent
                        var bulletins = db.tbl_bulletinSoin
                            .Where(b => b.idAdherent == adherent.idAdherent && b.isDeleted == false)
                            .ToList();

                        // Créer une instance de votre modèle Adherent et assigner les prestataires
                        // Créer une instance du modèle contenant à la fois l'adhérent et ses bulletins de soin
                        var viewModel = new DashboardViewModel
                        {
                            Adherent = adherent,
                        };

                        // Afficher la vue correspondante pour l'adhérent connecté avec les bulletins de soin
                        return View(viewModel); // Pas besoin de spécifier le nom de la vue ici
                    }
                }
            }
            return View();
        }



        /*  public ActionResult adhInfo()
          {
              if (Session["username"] != null)
              {
                  string username = Session["username"].ToString();

                  // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                  using (var db = new BsDBEntities25())
                  {
                      var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                      if (adherent != null)
                      {
                          // Charger les prestataires associés à cet adhérent
                          var prestataires = db.tbl_prestataire.Where(p => p.idAdherent == adherent.idAdherent && p.codePrestataire == "99").ToList();

                          // Ajouter les prestataires à la liste des prestataires de l'adhérent
                          adherent.Prestataires = prestataires;


                          // Créer une instance de votre modèle Adherent et assigner les bulletins de soin et les prestataires
                          var viewModel = new DashboardViewModel
                          {
                              Adherent = adherent,
                          };

                          // Afficher la vue correspondante pour l'adhérent connecté avec les bulletins de soin et les prestataires
                          return View(viewModel);
                      }
                  }
              }
              return View();
          }
        */

        public ActionResult adhInfo()
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();

                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities31())
                {
                    var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                    if (adherent != null)
                    {
                        // Charger les prestataires enfants associés à cet adhérent (code != "99")
                        var prestataires = db.tbl_prestataire.Where(p => p.idAdherent == adherent.idAdherent && p.codePrestataire != "99" && p.codePrestataire != "00").ToList();

                        // Charger les prestataires conjoints associés à cet adhérent (code == "99")
                        var prestatairesConjoint = db.tbl_prestataire.Where(p => p.idAdherent == adherent.idAdherent && p.codePrestataire == "99").ToList();

                        // Mapper les prestataires enfants et les prestataires conjoints vers le modèle Prestataire
                        var prestatairesConverted = prestataires.Select(p => new Prestataire
                        {
                            // Mapper les propriétés nécessaires
                            codePrestataire = p.codePrestataire,
                            prenomPres = p.prenomPres,
                            dateNais = p.dateNais,
                            sexe=p.sexe
                        }).ToList();

                        var prestatairesConvertedC = prestatairesConjoint.Select(p => new Prestataire
                        {
                            // Mapper les propriétés nécessaires
                            codePrestataire = p.codePrestataire,
                            nomPres = p.nomPres,
                            prenomPres = p.prenomPres,
                            dateNais = p.dateNais,
                            sexe=p.sexe,
                            profession=p.profession,
                            employeur = p.employeur,
                        }).ToList();

                        // Assigner les prestataires à la liste des prestataires de l'adhérent
                        adherent.Prestataires = prestatairesConverted;

                        // Assigner les prestataires de code "99" à la liste des prestataires conjoints de l'adhérent
                        adherent.PrestatairesConjoint = prestatairesConvertedC;

                        // Créer une instance de votre modèle Adherent et assigner les bulletins de soin et les prestataires
                        var viewModel = new DashboardViewModel
                        {
                            Adherent = adherent,
                        };

                        // Afficher la vue correspondante pour l'adhérent connecté avec les bulletins de soin et les prestataires
                        return View(viewModel);
                    }
                }
            }
            // Retourner la vue avec un modèle vide si l'utilisateur n'est pas connecté ou si l'adhérent n'est pas trouvé
            return View();
        }

        // Déclaration de filePath comme variable de classe
        private string filePath = "";


        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase file, long numBs)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();

                using (var db = new BsDBEntities31())
                {
                    tbl_bulletinSoin bs = new tbl_bulletinSoin();

                    string fileName = "";
                    string filePath = ""; // Utiliser une variable locale pour le chemin de fichier

                    // Récupérer le numéro de bulletin de soin depuis le paramètre numBs
                    string numBsString = numBs.ToString();

                    // Affectez le chemin du fichier téléchargé à la variable filePath
                    if (file != null && file.ContentLength > 0)
                    {
                        // Utiliser le numéro de bulletin de soin comme nom de fichier
                        fileName = numBsString + Path.GetExtension(file.FileName);
                        filePath = Path.Combine("C:\\Users\\wided\\source\\repos\\test2\\test2\\images", fileName);
                        file.SaveAs(filePath);
                    }

                    // Retournez le chemin du fichier dans la réponse JSON
                    return Json(new { success = true, bulletinId = bs.idBs, filePath = filePath });
                }
            }
            // Gérer les autres cas si nécessaire
            return Json(new { success = false });
        }


        public JsonResult saveUpdatedData(BulletinSoin model)
        {
            var result = false;
            var errors = "";

            if (Session["username"] != null)
            {
                using (var db = new BsDBEntities31())
                {
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

                                if (model.path != "KEEP_EXISTING_PATH")
                                {
                                    bs.path = model.path;
                                }

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
                }
            }
            else
            {
                errors = "Session expirée ou utilisateur non authentifié.";
            }

            return Json(new { success = result, errors = errors }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDataInDatabase(BulletinSoin model, Adherent model2, List<Rubrique1Model> Rubriques, List<Rubrique2Model> Rubriques2, List<Rubrique3Model> Rubriques3)
        {
            var result = false;
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();


                using (var db = new BsDBEntities31())

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
                        bs.numBs = model.numBs;

                        // Mettre à jour les propriétés du bulletin de soin
                        bs.idAdherent = model.idAdherent;
                        bs.idPrestataire = model.idPrestataire;

                        // Mettre à jour les propriétés de l'adhérent
                        adh.nomAdh = model.nomAdh;
                        adh.prenomAdh = model.prenomAdh;
                        adh.matriculeCnam = model.matriculeCnam;
                        adh.CIN_PASS = model.CIN_PASS;
                        adh.adressAdh = model.adressAdh;
                        bs.path = model.path;

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
                        bs.isDeleted = false;
                        bs.status = "new";

                        // Définir le chemin du fichier dans model.path
                        bs.path = model.path; // Assurez-vous que model.path contient le chemin de la photo

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
                                        montant_estimé = rubrique.montant_estimé,
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
                        // Exemple d'utilisation :
                      
     
                    }
               
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
       
        public JsonResult UploadImage()
        {
            var result = false;
            var imagePath = "";

            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(file.FileName);
                        string relativePath = "~/bs_images/" + filename; // Chemin relatif
                        string path = Server.MapPath(relativePath); // Chemin physique
                        file.SaveAs(path);
                        imagePath = relativePath; // Stockez le chemin relatif de l'image dans votre base de données ou utilisez-le comme nécessaire
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou prendre des mesures appropriées
                Console.WriteLine("Error uploading image: " + ex.Message);
            }

            return Json(new { success = result, imagePath = imagePath });
        }

        [HttpGet]
        public JsonResult GetAdherentDetails(int idAdherent)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();

                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities31())
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

                        if (model != null)
                        {
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { error = "L'adhérent n'a pas été trouvé dans la base de données." });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logguer l'exception ou envoyer un rapport d'erreur
                        // Vous pouvez également renvoyer un message d'erreur spécifique
                        return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
                    }

                }
            }
            else
            {
                return Json(new { error = "La session utilisateur n'est pas valide." });
            }
        }

        [HttpGet]

        public JsonResult GetBsList(int page = 1, int pageSize = 5)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();

                // Rechercher l'adhérent dans la base de données en utilisant son nom d'utilisateur
                using (var db = new BsDBEntities31())
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

                        // Récupérer l'ID de l'adhérent associé au nom d'utilisateur de la session
                        var adherent = db.tbl_adherent.FirstOrDefault(a => a.idAdherent.ToString() == username);
                        if (adherent != null)
                        {
                            var bsList = db.tbl_bulletinSoin
                            .Where(x => x.isDeleted == false && x.idAdherent == adherent.idAdherent)
                            .OrderBy(x => x.idBs)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(x => new BulletinSoin
                            {
                                idBs = x.idBs,
                                numBs = x.numBs,

                                status = x.status
                                // Ajoutez d'autres propriétés à mapper si nécessaire
                            })
                            .ToList();

                            // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                            int totalRecords = bsList.Count;

                            return Json(bsList, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { error = "Aucun adhérent trouvé pour le nom d'utilisateur de la session." });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logguer l'exception ou envoyer un rapport d'erreur
                        // Vous pouvez également renvoyer un message d'erreur spécifique
                        return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
                    }
                }
            }
            else
            {
                // Gérer le cas où "username" dans Session est null
                return Json(new { error = "Le nom d'utilisateur n'est pas défini dans la session." });
            }
        }


        public JsonResult GetbsNewById(int idBs)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
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
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }

        // Méthode pour récupérer les rubriques associées à un bulletin de soin
        public JsonResult GetRubriquesByIdBs(int idBs)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
                    {
                        var rubriques = db.tbl_rub1
                        .Where(r => r.idBs == idBs)
                        .Select(r => new RubriqueViewModel
                        {
                            idRub1 = r.idRub1,
                            dateRub1 = (DateTime)r.dateRub1,
                            codeNom = r.tbl_sousFamille.codeNom,
                            nomSousFamille = r.tbl_sousFamille.nomSousFamille,
                            honorairesRub1 = r.honorairesRub1.HasValue ? (decimal)r.honorairesRub1 : 0, // Gérer les valeurs null
                            deviseRub1 = r.deviseRub1,
                            montant_change = r.montant_change,
                            montant_estimé = r.montant_estimé,
                            matriculeFisacalRub1 = r.matriculeFisacalRub1,
                            // Ajoutez d'autres propriétés ici
                        })
                        .ToList();


                        return Json(rubriques, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Une erreur s'est produite lors de la récupération des rubriques." });
            }
        }

        // Méthode pour récupérer les rubriques associées à un bulletin de soin
        public JsonResult GetRubriques2ByIdBs(int idBs)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
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
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
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
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la synchronisation des données." });
            }
        }

        // Les méthodes SynchroniserRubriques2 et SynchroniserRubriques3 peuvent être modifiées de la même manière.

        [HttpPost]
        public ActionResult SynchroniserRubriques2(Rubrique2Model[] rubriquesData2)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
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
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
                    {
                        if (rubriquesData3 != null && rubriquesData3.Length > 0)
                        {
                            foreach (var rubriqueData3 in rubriquesData3)
                            {
                                // Créez une nouvelle instance de votre modèle Rubrique3Model
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la synchronisation des données." });
            }
        }




        public ActionResult DeleteRubrique(int idRubrique) 
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la suppression de la rubrique de la base de données." });
            }
        }

        [HttpPost]
        public ActionResult DeleteRubrique2(int idRubrique2)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la suppression de la rubrique de la base de données." });
            }
        }

        [HttpPost]
        public ActionResult DeleteRubrique3(int idRubrique3)
        {
            try
            {
                if (Session["username"] != null)
                {
                    string username = Session["username"].ToString();
                    using (var db = new BsDBEntities31())
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
                }
                else
                {
                    return Json(new { error = "Session 'username' non définie." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Une erreur s'est produite lors de la suppression de la rubrique de la base de données." });
            }
        }





        [HttpGet]
        public JsonResult GetTauxDeChange(string selectedCurrency, DateTime rubriqueDate, Rubrique1Model rubriqueModel, tauxChange tauxModel)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();
                using (var db = new BsDBEntities31())
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
            }
            else
            {
                // Si l'utilisateur n'est pas connecté, retournez une erreur
                return Json(new { error = "L'utilisateur n'est pas connecté." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetValeurByIdSousFamille(int idSousFamille)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();
                using (var db = new BsDBEntities31())
                {
                    var sf = db.tbl_sousFamille
                        .Where(s => s.idSousFamille == idSousFamille && s.isDeleted == false)
                        .Select(s => new
                        {
                            Valeur = s.valeur,
                            Unite = s.unité,
                            Max = s.max
                        })
                        .FirstOrDefault();

                    return Json(sf, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                // Si l'utilisateur n'est pas connecté, retournez une erreur
                return Json(new { error = "L'utilisateur n'est pas connecté." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetTauxDeChange2(string selectedCurrency, DateTime rubriqueDate, Rubrique2Model rubriqueModel, tauxChange tauxModel)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();
                using (var db = new BsDBEntities31())
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
            }
            else
            {
                // Si l'utilisateur n'est pas connecté, retournez une erreur
                return Json(new { error = "L'utilisateur n'est pas connecté." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTauxDeChange3(string selectedCurrency, DateTime rubriqueDate, Rubrique3Model rubriqueModel, tauxChange tauxModel)
        {
            if (Session["username"] != null)
            {
                string username = Session["username"].ToString();
                using (var db = new BsDBEntities31())
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
            }
            else
            {
                // Si l'utilisateur n'est pas connecté, retournez une erreur
                return Json(new { error = "L'utilisateur n'est pas connecté." }, JsonRequestBehavior.AllowGet);
            }
        }







    }
}