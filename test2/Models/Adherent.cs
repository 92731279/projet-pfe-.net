using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Adherent
    {
        public int idAdherent { get; set; }
        public string nomAdh { get; set; }
        public string prenomAdh { get; set; }
        public Nullable<long> CIN_PASS { get; set; }
        public string adressAdh { get; set; }
        public string emailAdh { get; set; }
        public Nullable<long> matriculeCnam { get; set; }
        public string status { get; set; }
        public Nullable<long> numTelephone { get; set; }
        public bool isDeleted { get; set; }
        public int police { get; set; }
        public System.DateTime dateAdhesion { get; set; }
        public Nullable<System.DateTime> dateNais { get; set; }
        public string lieuNais { get; set; }
        public string sexeAdherent { get; set; }
        public string situationFamiliale { get; set; }
        public string gouvernorat { get; set; }
        public Nullable<int> codePostal { get; set; }
        public string profession { get; set; }
        public string lieu { get; set; }
        public Nullable<double> salaireBrut { get; set; }
        public Nullable<long> RIB { get; set; }
        public string benefEnCasDeces { get; set; }

        public Prestataire pres { get; set; }

        public List<Prestataire> PrestatairesConjoint { get; set; }

        public List<Prestataire> Prestataires { get; set; }

        public List<BulletinSoin> BulletinsSoins { get; set; }

    }
}