using Client.Player;
using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField] TextMeshProUGUI health;

    public void Init( PlayerController owner )
    {
        playerController = owner;

        UpdateHealth();
        playerController.OnHealthChange += UpdateHealth;
    }

    void UpdateHealth()
    {
        health.text = $"{playerController.health.Value}";
    }
}
