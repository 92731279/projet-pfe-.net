using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test2.Models
{
    public class Famille
    {
        public int idFamille { get; set; }
        public string nomFamille { get; set; }
        public string obsFamille { get; set; }
        public bool isDeleted { get; set; }
    }
}