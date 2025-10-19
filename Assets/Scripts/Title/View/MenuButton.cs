using Title.Service;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Title.View
{
    public class MenuButton : MonoBehaviour
    {
        MenuButtonClickService _menuButtonClickService;
        [SerializeField] Button _button;
        [SerializeField] MenuButtonClickService.MenuButtonType _menuButtonType;

        [Inject]
        public void Init(MenuButtonClickService menuButtonClickService)
        {
            _menuButtonClickService = menuButtonClickService;
        }

        void Awake()
        {
            _button.onClick.AddListener(Click);
        }

        void Click()
        {
            _menuButtonClickService.Click(_menuButtonType);
        }
    }
}
