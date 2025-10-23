using UnityEngine;

namespace Root.Service
{
    public class EditorLogger : ILogger
    {
        public void Log(string content)
        {
            Debug.Log(content);
        }

        public void LogWarning(string content)
        {
            Debug.LogWarning(content);
        }

        public void LogError(string content)
        {
            Debug.LogError(content);
        }
    }
}
