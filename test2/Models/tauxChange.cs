using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class tauxChange
    {
        public string devise_locale { get; set; }
        public string devise_etrangere { get; set; }
        public System.DateTime date_debut { get; set; }
        public System.DateTime date_fin { get; set; }
        public double taux { get; set; }
        public bool is_deleted { get; set; }
        public int id { get; set; }

    }

}