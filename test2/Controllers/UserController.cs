using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test2.Models;

namespace test2.Controllers
{
    public class UserController : Controller
    {


        // GET: User
        BsDBEntities31 db4 = new BsDBEntities31();

        public ActionResult HomePage()
        {
            return View();
        }
        public ActionResult tables()
        {
            return View();
        }

        public JsonResult GetUserList(int page = 1, int pageSize = 5)
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

                var userList = db4.tbl_login
                    .Where(x => x.isDeleted == false)
                    .OrderBy(x => x.id) // Assurez-vous de trier les données pour une pagination cohérente
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()  // Matérialise les résultats intermédiaires en mémoire
                    .Select(x => new Account
                    {
                        id = x.id,
                        nom = x.nom,
                        prenom = x.prenom,
                        Name = x.username,
                        password = x.paassword,
                        role = x.role,
                    }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db4.tbl_login.Count(x => x.isDeleted == false);

                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetUserList2(int page = 1, int pageSize = 5)
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

                var userList = db4.tbl_login
                   .Where(x => x.isDeleted == false)
                   .OrderBy(x => x.id) // Assurez-vous de trier les données pour une pagination cohérente
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList()  // Matérialise les résultats intermédiaires en mémoire
                   .Select(x => new Account
                   {
                       id = x.id,
                       nom = x.nom,
                       prenom = x.prenom,
                       Name = x.username,
                       password = x.paassword,
                       role = x.role,
                   }).ToList();

                // Vous pouvez également obtenir le nombre total d'enregistrements pour calculer le nombre total de pages
                int totalRecords = db4.tbl_login.Count(x => x.isDeleted == false);

                return Json(new { data = userList, totalRecords = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                // Vous pouvez également renvoyer un message d'erreur spécifique
                return Json(new { error = "Une erreur s'est produite lors de la récupération des données." });
            }
        }


        public JsonResult GetUserById(int id)
        {
            tbl_login model = db4.tbl_login.Where(x => x.id == id).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult SaveDataInDatabase(Account model)
        {
            var result = false;
            try
            {

                if (model.id > 0)
                {
                    tbl_login user = db4.tbl_login.SingleOrDefault(x => x.isDeleted == false && x.id == model.id);
                    user.id = model.id;
                    user.nom = model.nom;
                    user.prenom = model.prenom;
                    user.username = model.Name;
                    user.paassword = model.password;
                    user.role = model.role;

                    db4.SaveChanges();
                    result = true;
                }
                else
                {
                    tbl_login user = new tbl_login();
                    user.id= model.id;
                    user.nom = model.nom;
                    user.prenom = model.prenom;
                    user.username = model.Name;
                    user.paassword = model.password;
                    user.role = model.role;

                    db4.tbl_login.Add(user);
                    db4.SaveChanges();
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

        public JsonResult DeleteUserRecord(int id)
        {
            bool result = false;
            tbl_login user = db4.tbl_login.SingleOrDefault(x => x.isDeleted == false && x.id == id);
            if (user != null)
            {
                user.isDeleted = true;
                db4.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUserStatistics()
        {
            try
            {
                // Nombre total d'utilisateurs
                int totalUsers = db4.tbl_login.Count(x => x.isDeleted == false);

                // Nombre d'utilisateurs par rôle
                var usersByRole = db4.tbl_login
                    .Where(x => x.isDeleted == false)
                    .GroupBy(x => x.role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToList();

                
                var statistics = new
                {
                    TotalUsers = totalUsers,
                    UsersByRole = usersByRole,
                    
                };

                return Json(statistics, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Logguer l'exception ou envoyer un rapport d'erreur
                return Json(new { error = "Une erreur s'est produite lors de la récupération des statistiques." });
            }
        }

        public ActionResult UserDash()
        {
            return View();
        }


    }
}