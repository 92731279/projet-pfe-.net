using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class RubriqueViewModel
    {
        public int idRub1 { get; set; }
        public System.DateTime dateRub1 { get; set; }
        public int designationId { get; set; }
        public decimal honorairesRub1 { get; set; }
        public string deviseRub1 { get; set; }
        public string matriculeFisacalRub1 { get; set; }

        public int idBs { get; set; }
        
        public string designationNo { get; set; }
        public string designationNom { get; set; }
        public string nomSousFamille { get; set; }
        public string codeNom { get; set; }



        public Nullable<int> idSousFamille { get; set; }
        public Nullable<double> montant_estimé { get; set; }




        public string devise_souhaite { get; set; }
        public Nullable<double> montant_change { get; set; }
        public int id { get; set; }

        public int idRub2 { get; set; }
        public Nullable<System.DateTime> DE { get; set; }
        public Nullable<System.DateTime> DS { get; set; }
        public Nullable<decimal> montantFrais { get; set; }
        public string deviseRub2 { get; set; }
        public string matriculeFiscalRub2 { get; set; }
        public Nullable<double> montant_change2 { get; set; }
        public Nullable<double> montant_estimé2 { get; set; }

        public int idRub3 { get; set; }
        public Nullable<System.DateTime> dateRub3 { get; set; }
        public string dents { get; set; }
        public string actMedical { get; set; }
        public Nullable<decimal> honorairesRub3 { get; set; }
        public string deviseRub3 { get; set; }
        public string matriculeFiscalRub3 { get; set; }
        public string type { get; set; }
        public Nullable<double> montant_change3 { get; set; }
        public Nullable<double> montant_estimé3 { get; set; }



    }
}