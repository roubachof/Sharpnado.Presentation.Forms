using System;
using System.Diagnostics;

namespace Sharpnado.Presentation.Forms
{
    public static class InternalLogger
    {
        public static bool EnableLogging { get; set; } = false;

        public static void Info(string format, params object[] parameters)
        {
            DiagnosticLog("INFO | " + format, parameters);
        }

        public static void Warn(string format, params object[] parameters)
        {
            DiagnosticLog("WARN | " + format, parameters);
        }

        public static void Error(string format, params object[] parameters)
        {
            DiagnosticLog("ERROR | " + format, parameters);
        }

        public static void Error(Exception exception)
        {
            Error($"{exception.Message}{Environment.NewLine}{exception}");
        }

        private static void DiagnosticLog(string format, params object[] parameters)
        {
            if (!EnableLogging)
            {
                return;
            }

#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString("MM-dd H:mm:ss.fff") + " | SharpnadoInternals | " + format, parameters);
#else
            Console.WriteLine(DateTime.Now.ToString("MM-dd H:mm:ss.fff") + " | SharpnadoInternals | " + format, parameters);
#endif
        }
    }
}