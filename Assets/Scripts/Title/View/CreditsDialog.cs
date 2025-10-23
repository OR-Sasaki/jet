using Root.View;
using TMPro;
using UnityEngine;

namespace Title.View
{
    /// <summary>
    /// クレジット表示ダイアログ
    /// </summary>
    public class CreditsDialog : Dialog
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _contentText;

        protected override void Awake()
        {
            base.Awake();

            if (_titleText != null)
            {
                _titleText.text = "Credits";
            }

            if (_contentText != null)
            {
                _contentText.text = "Game Developer: Hemuichi\n\n" +
                                    "Level Design: Ogu";
            }
        }
    }
}

