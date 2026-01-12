using Client.Player;
using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] TextMeshProUGUI id;
    [SerializeField] TextMeshProUGUI health;

    public void Init( PlayerController owner )
    {
        playerController = owner;

        UpdateHealth();
        SetId();
        playerController.OnHealthChange += UpdateHealth;
    }

    void SetId()
    {
        id.text = $"{playerController.CharacterNetId}";
    }

    void UpdateHealth()
    {
        health.text = $"{playerController.health.Value}";
    }
}
