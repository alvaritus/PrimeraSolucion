﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADA.Despacho.CommonEntities;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication1
{
    //http://andy.edinborough.org/Reading-Entity-Framework-Code-First-Objects-from-a-Stored-Procedure
    //http://www.c-sharpcorner.com/UploadFile/ff2f08/call-store-procedure-from-entity-framework/

    class Program
    {
        public const string DespachoDbConnectionStringName = "DespachoConnectionString";

        //Definir el Logger
        static ILogger Logger { get; } = ApplicationLogging.CreateLogger<Program>();

        static void Main(string[] args)
        {
            ApplicationLogging.InicializarApplicationLogging();

            using (Logger.BeginScopeImpl(nameof(Main)))
            {
                Logger.LogInformation("Comienza Main");
                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        var datos = ObtenerRecepcionesTelefonicas();
                        sw.Stop();
                        var elapsedconentlib = sw.Elapsed.TotalSeconds;
                        sw.Reset();
                        sw.Start();
                        List<RecepcionTelefonica> rts;
                        using (DespachoEntities context = new DespachoEntities())
                        {
                            rts = context.ObtenerRecepcionTelefonicas(System.Data.Entity.Core.Objects.MergeOption.NoTracking).ToList();
                        }
                        sw.Stop();
                        var elapsedef = sw.Elapsed.TotalSeconds;
                        sw.Reset();
                        sw.Start();
                        var datos3 = LeerSQLDataReader();
                        sw.Stop();
                        var elapsedefsqldr = sw.Elapsed.TotalSeconds;
                        var sb = new StringBuilder();
                        sb.AppendLine("Iteración: " + (i + 1).ToString());
                        sb.AppendLine("Con Entlib: " + elapsedconentlib.ToString());
                        sb.AppendLine("Con EF: " + elapsedef.ToString());
                        sb.AppendLine("Con EF SQL DATA READER: " + elapsedefsqldr.ToString());
                        Logger.LogInformation(sb.ToString());
                        throw new Exception("Error al realizar el test");
                    }
                    Console.WriteLine("Presione una tecla para terminar");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Logger.LogCritical("Se ha producido un error en Main", ex);
                }
                Logger.LogInformation("Termina Main");
            }
        }

        /// <summary>
        /// Returns the set name for a given entity type (http://social.msdn.microsoft.com/Forums/en-US/adodotnetentityframework/thread/7a29d4e3-8550-43dd-aa09-2bb859466c0d)
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        private static string GetEntitySetName<T>(ObjectContext o)
        {
            return o.MetadataWorkspace.GetEntityContainer(o.DefaultContainerName, DataSpace.CSpace).BaseEntitySets.FirstOrDefault(bes => bes.ElementType.Name == typeof(T).Name).Name;
        }

        public static List<RecepcionTelefonica> LeerSQLDataReader()
        {
            List<RecepcionTelefonica> rts = null;
            using (DespachoEntities context = new DespachoEntities())
            {
                //rts = context.ObtenerRecepcionTelefonicas().ToList();

                string connectionString = ConfigurationManager.ConnectionStrings["DespachoConnectionString"].ConnectionString; //(context.Database.Connection as EntityConnection).StoreConnection.ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = "ObtenerRecepcionTelefonicas";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            //cmd.CommandTimeout = 0;
                            var sqlReader = (DbDataReader)cmd.ExecuteReader();
                            var adapter = (IObjectContextAdapter)context;
                            var objectContext = adapter.ObjectContext;
                            //var entitySetName = GetEntitySetName<RecepcionTelefonica>(objectContext);
                            rts = objectContext.Translate<RecepcionTelefonica>(sqlReader, "RecepcionTelefonica", MergeOption.NoTracking).ToList();
                        }
                        catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
                        if (con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
            return rts;
        }

        public static Microsoft.Practices.EnterpriseLibrary.Data.Database GetDataBaseDespacho()
        {
            try
            {
                // Configure the DatabaseFactory to read its configuration from the .config file 
                var factory = new DatabaseProviderFactory();

                // Create the default Database object from the factory. 
                // The actual concrete type is determined by the configuration settings. 
                //var defaultDB = factory.CreateDefault(); 

                // Create a Database object from the factory using the connection string name. 
                var denunciaDb = factory.Create(DespachoDbConnectionStringName);

                return denunciaDb;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<RecepcionTelefonica> ObtenerRecepcionesTelefonicas()
        {
            try
            {
                return GetDataBaseDespacho().ExecuteSprocAccessor<RecepcionTelefonica>("ObtenerRecepcionTelefonicas").ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }



}
