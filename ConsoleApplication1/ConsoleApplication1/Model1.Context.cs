﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DespachoEntities : DbContext
    {
        public DespachoEntities()
            : base("name=DespachoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<RecepcionTelefonica> RecepcionTelefonica { get; set; }
        public virtual DbSet<RecepcionTelefonicaAuditoriaDeEstados> RecepcionTelefonicaAuditoriaDeEstados { get; set; }
    
        public virtual ObjectResult<RecepcionTelefonica> ObtenerRecepcionTelefonicas()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RecepcionTelefonica>("ObtenerRecepcionTelefonicas");
        }
    
        public virtual ObjectResult<RecepcionTelefonica> ObtenerRecepcionTelefonicas(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RecepcionTelefonica>("ObtenerRecepcionTelefonicas", mergeOption);
        }
    
        public virtual ObjectResult<RecepcionTelefonica> ObtenerRecepcionesTelefonicas()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RecepcionTelefonica>("ObtenerRecepcionesTelefonicas");
        }
    
        public virtual ObjectResult<RecepcionTelefonica> ObtenerRecepcionesTelefonicas(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<RecepcionTelefonica>("ObtenerRecepcionesTelefonicas", mergeOption);
        }
    }
}
