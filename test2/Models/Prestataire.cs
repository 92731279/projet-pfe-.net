using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Prestataire
    {
        public int idPrestataire { get; set; }
        public string codePrestataire { get; set; }
        public string nomPres { get; set; }
        public string prenomPres { get; set; }
        public Nullable<System.DateTime> dateNais { get; set; }
        public string adressPres { get; set; }
        public int idAdherent { get; set; }
        public string sexe { get; set; }
        public string profession { get; set; }
        public string employeur { get; set; }


    }
}