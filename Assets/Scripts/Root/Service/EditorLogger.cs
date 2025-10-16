using UnityEngine;

namespace Root.Service
{
    public class EditorLogger : ILogger
    {
        public void Log(string content)
        {
            Debug.Log(content);
        }
    }
}
