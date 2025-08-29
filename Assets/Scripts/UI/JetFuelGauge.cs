using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JetFuelGauge : MonoBehaviour
{
    [SerializeField] TMP_Text jetFuelText;
    [SerializeField] Image jetFuelGaugeImage;
    [SerializeField] Image BombImage;

    Player player;

    void Start()
    {
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
    }

    void Update()
    {
        SetFuel(GameSettings.I.MaxJetFuel, player.remainJetFuel);
        SetBomb(player.canBomb);
    }

    void SetFuel(float maxJetFuel, float remainJetFuel)
    {
        jetFuelText.text = $"{remainJetFuel:F1}";
        jetFuelGaugeImage.fillAmount = remainJetFuel / maxJetFuel;
    }

    void SetBomb(bool canBomb)
    {
        BombImage.enabled = canBomb;
    }
}
