using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Rubrique3Model
    {
        public int idRub3 { get; set; }
        public Nullable<System.DateTime> dateRub3 { get; set; }
        public string dents { get; set; }
        public string actMedical { get; set; }
        public Nullable<decimal> honorairesRub3 { get; set; }
        public string deviseRub3 { get; set; }
        public string matriculeFiscalRub3 { get; set; }
        public string type { get; set; }
        public int idBs { get; set; }
        public Nullable<double> montant_change3 { get; set; }
        public Nullable<double> montant_estimé3 { get; set; }

    }
}