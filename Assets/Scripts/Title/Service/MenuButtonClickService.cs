using System;
using Root.Manager;
using Root.Service;
using Title.View;
using UnityEngine;

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
        readonly DialogManager _dialogManager;

        public MenuButtonClickService(SceneLoader sceneLoader, DialogManager dialogManager)
        {
            _sceneLoader = sceneLoader;
            _dialogManager = dialogManager;
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
                    _dialogManager.OpenDialog<SettingsDialog>();
                    break;
                case MenuButtonType.Credits:
                    _dialogManager.OpenDialog<CreditsDialog>();
                    break;
            }
        }
    }
}
