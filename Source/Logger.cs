using UnityEngine;

namespace CombatAgent
{
    public class SuppressLogHandler : ILogHandler
    {
        private ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);

            // Suppress specific messages
            if (message.Contains("R32_SFloat") ||
                message.Contains("Texture") ||
                message.Contains("Verse.SectionLayer") ||
                message.Contains("Verse.SectionLayer"))
            {
                return;
            }



            // Log other messages normally
            defaultLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(System.Exception exception, UnityEngine.Object context)
        {
            // Suppress specific exceptions
            if (context is Verse.CameraDriver)
            {
                return;
            }

            // Log other exceptions normally with context information
            defaultLogHandler.LogException(exception, context);
        }
    }
}