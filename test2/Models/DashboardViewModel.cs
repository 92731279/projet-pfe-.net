using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class DashboardViewModel
    {
        public tbl_adherent Adherent { get; set; }
        public List<BulletinSoin> BulletinSoin { get; set; }
        public List<BulletinSoin> BulletinSoinNew { get; set; }

        public BulletinSoin bulletinSoin { get; set; }

        public List<Prestataire> Prestataires { get; set; }
        public List<Prestataire> PrestatairesEnfants { get; set; }
        public List<Prestataire> PrestatairesConjoint { get; set; }


        public DashboardViewModel()
        {
            Prestataires = new List<Prestataire>();
            PrestatairesConjoint = new List<Prestataire>();
        }


    }

}