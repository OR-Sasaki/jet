using EntryPointScene.Manager;
using VContainer.Unity;

namespace EntryPointScene.Starter
{
    public class EntryPointStarter : IStartable
    {
        readonly EntryPointManager _manager;

        public EntryPointStarter(EntryPointManager manager)
        {
            _manager = manager;
        }

        public void Start()
        {
            _manager.Start();
        }
    }
}
