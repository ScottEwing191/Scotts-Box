using UnityEngine;

namespace ScottEwing.Scotts_Box.Utilities{
    public static class Debug{
        public static bool DebugMode = true;
        public static void Log(string message) {
            if (DebugMode)
                UnityEngine.Debug.Log(message);
        }

        public static void Log(string message, Object context) {
            if (DebugMode)
                UnityEngine.Debug.Log(message, context);
        }

        public static void LogWarning(string message) {
            if (DebugMode)
                UnityEngine.Debug.LogWarning(message);
        }

        public static void LogWarning(string message, Object context) {
            if (DebugMode)
                UnityEngine.Debug.LogWarning(message, context);
        }

        public static void LogError(string message) {
            if (DebugMode)
                UnityEngine.Debug.LogError(message);
        }

        public static void LogError(string message, Object context) {
            if (DebugMode)
                UnityEngine.Debug.LogError(message, context);
        }

    }
}