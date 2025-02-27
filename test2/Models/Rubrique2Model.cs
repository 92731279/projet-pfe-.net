using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Rubrique2Model
    {
        public int idRub2 { get; set; }
        public Nullable<System.DateTime> DE { get; set; }
        public Nullable<System.DateTime> DS { get; set; }
        public Nullable<decimal> montantFrais { get; set; }
        public string deviseRub2 { get; set; }
        public string matriculeFiscalRub2 { get; set; }
        public int idBs { get; set; }
        public Nullable<double> montant_change2 { get; set; }
        public Nullable<double> montant_estimé2 { get; set; }


    }
}