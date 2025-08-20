using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JetFuelGauge : MonoBehaviour
{
    [SerializeField] TMP_Text jetFuelText;
    [SerializeField] Image jetFuelGaugeImage;

    Player player;

    void Start()
    {
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
    }

    void Update()
    {
        Set(player.maxJetFuel, player.remainJetFuel);
    }

    void Set(float maxJetFuel, float remainJetFuel)
    {
        jetFuelText.text = $"{remainJetFuel:F1}";
        jetFuelGaugeImage.fillAmount = remainJetFuel / maxJetFuel;
    }
}
