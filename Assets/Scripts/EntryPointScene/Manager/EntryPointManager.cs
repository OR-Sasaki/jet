using Root.Service;

namespace EntryPointScene.Manager
{
    public class EntryPointManager
    {
        SceneLoader _sceneLoader;

        public EntryPointManager(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Start()
        {
            _sceneLoader.LoadScene("Title");
        }
    }
}
