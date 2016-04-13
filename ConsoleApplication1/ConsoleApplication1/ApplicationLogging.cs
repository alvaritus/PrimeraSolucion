using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace ConsoleApplication1
{
    // https://msdn.microsoft.com/es-es/magazine/mt694089
    // https://msdn.microsoft.com/es-es/magazine/mt694089
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        public static void InicializarApplicationLogging()
        {
            EventLogSettings settings = new EventLogSettings();
            settings.LogName = "10_ADA_Cliente15_Despacho";
            settings.SourceName = "10_ADA_Cliente15_Despacho";
            LoggerFactory.AddEventLog(settings);
           // LoggerFactory.AddConsole(true);
            LoggerFactory.AddDebug();
        }
        
    }
}
