using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class BulletinSoinNew
    {
        public int idBs { get; set; }
        public bool isDeleted { get; set; }
        public int idAdherent { get; set; }
        public int idPrestataire { get; set; }
        public string status { get; set; }
        public long numBs { get; set; }
        public string path { get; set; }
        public string nomAdh { get; set; }
        public string prenomAdh { get; set; }
        public long matriculeCnam { get; set; } // Utilisation d'un type nullable
        public long CIN_PASS { get; set; } // Utilisation d'un type nullable
        public string adressAdh { get; set; }

        public string codePrestataire { get; set; }
        // Ajoutez les propriétés des rubriques
        public string nomPres { get; set; }
        public string prenomPres { get; set; }
        public RubriqueViewModel Rubrique1 { get; set; }
        public RubriqueViewModel Rubrique2 { get; set; }
        public RubriqueViewModel Rubrique3 { get; set; }
        public List<RubriqueViewModel> Rubrique { get; set; }
        public List<RubriqueViewModel> Rubrique_2 { get; set; }
        public List<RubriqueViewModel> Rubrique_3 { get; set; }

    }
}