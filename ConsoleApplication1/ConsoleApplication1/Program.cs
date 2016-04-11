using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADA.Despacho.CommonEntities;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        public const string DespachoDbConnectionStringName = "DespachoConnectionString";

        static void Main(string[] args)
        {
            for (int i = 0; i < 1001; i++)
            {
                Console.WriteLine("Iteración: " + (i + 1).ToString());
                var sw = new Stopwatch();
                sw.Start();
                var datos = ObtenerRecepcionesTelefonicas();
                sw.Stop();
                var elapsed = sw.Elapsed.TotalSeconds;
                Console.WriteLine("Con Entlib: " + elapsed.ToString());
                sw.Reset();
                sw.Start();
                List<RecepcionTelefonica> rts;
                using (DespachoEntities context = new DespachoEntities())
                {
                    rts = context.ObtenerRecepcionTelefonicas().ToList();
                }
                sw.Stop();
                elapsed = sw.Elapsed.TotalSeconds;
                Console.WriteLine("Con EF: " + elapsed.ToString());
                sw.Reset();
                sw.Start();
                var datos3 = LeerSQLDataReader();
                sw.Stop();
                elapsed = sw.Elapsed.TotalSeconds;
                Console.WriteLine("Con EF SQL DATA READER: " + elapsed.ToString());
            }
            //Console.WriteLine("Hola mundo");
            Console.ReadKey();
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
                        rts = objectContext.Translate<RecepcionTelefonica>(sqlReader).ToList();
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
