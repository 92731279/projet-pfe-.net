using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Rubrique1Model
    {
        public int idRub1 { get; set; }
        public Nullable<System.DateTime> dateRub1 { get; set; }
        public Nullable<int> designationId { get; set; }
        public Nullable<decimal> honorairesRub1 { get; set; }
        public string deviseRub1 { get; set; }
        public string matriculeFisacalRub1 { get; set; }
        public int idBs { get; set; }
        public string devise_souhaite { get; set; }
        public Nullable<double> montant_change { get; set; }
        public Nullable<int> idSousFamille { get; set; }
        public Nullable<double> montant_estimé { get; set; }

        public string nomSousFamille { get; set; }
        public string codeNom { get; set; }


    }
}