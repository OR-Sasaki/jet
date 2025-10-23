using UnityEngine;

namespace Root.Service
{
    public interface ILogger
    {
        public void Log(string content);
        public void LogWarning(string content);
        public void LogError(string content);
    }
}
