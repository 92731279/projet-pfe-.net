﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace test2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BsDBEntities17 : DbContext
    {
        public BsDBEntities17()
            : base("name=BsDBEntities17")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_adherent> tbl_adherent { get; set; }
        public virtual DbSet<tbl_bulletinSoin> tbl_bulletinSoin { get; set; }
        public virtual DbSet<tbl_designation> tbl_designation { get; set; }
        public virtual DbSet<tbl_prestataire> tbl_prestataire { get; set; }
        public virtual DbSet<tbl_rub1> tbl_rub1 { get; set; }
        public virtual DbSet<tbl_rub2> tbl_rub2 { get; set; }
        public virtual DbSet<tbl_rub3> tbl_rub3 { get; set; }
        public virtual DbSet<tbl_taux_de_change> tbl_taux_de_change { get; set; }
        public virtual DbSet<tbl_login> tbl_login { get; set; }
    }
}
