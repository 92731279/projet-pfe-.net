using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace test2.Controllers
{
    public class AdhérentController : Controller
    {
        // GET: Adherent/Details/5
        public ActionResult Details(int id)
        {
            // Ici, vous pouvez récupérer les détails de l'adhérent à partir de l'ID
            // Par exemple, vous pouvez utiliser cet ID pour interroger la base de données et récupérer les informations nécessaires

            // Supposons que vous avez une classe de service qui récupère les détails de l'adhérent en fonction de son ID
            // AdherentService service = new AdherentService();
            // AdherentDetailsViewModel model = service.GetAdherentDetails(id);

            // Pour cet exemple, nous allons simplement afficher l'ID de l'adhérent dans la vue
            ViewBag.AdherentId = id;

            // Retourne la vue avec l'ID de l'adhérent
            return View();
        }
    }
}