
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class sousFamille
    {
        public int idSousFamille { get; set; }
        public string nomSousFamille { get; set; }
        public double valeur { get; set; }
        public string unité { get; set; }
        public Nullable<double> max { get; set; }
        public string unitéMax { get; set; }
        public string obsSousFamille { get; set; }
        public int idFamille { get; set; }
        public bool isDeleted { get; set; }
        public string codeNom { get; set; }
        public string nomFamille { get; set; }
        public  Famille famille { get; set; }


    }
}