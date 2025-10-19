using System;
using Root.Service;

namespace Title.Service
{
    public class MenuButtonClickService
    {
        public enum MenuButtonType
        {
            Singleplayer,
            Multiplayer,
            Settings,
            Credits
        }

        readonly SceneLoader _sceneLoader;

        public MenuButtonClickService(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Click(MenuButtonType menuButtonType)
        {
            switch (menuButtonType)
            {
                case MenuButtonType.Singleplayer:
                    _sceneLoader.LoadScene("Game");
                    break;
                case MenuButtonType.Multiplayer:
                    break;
                case MenuButtonType.Settings:
                    break;
                case MenuButtonType.Credits:
                    break;
            }
        }
    }
}
