using Client.Player;
using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    EntityController targetEntity;
    CharacterStats characterStats;
    [SerializeField] TextMeshProUGUI id;
    [SerializeField] TextMeshProUGUI health;

    public void Init( GameObject targetCharacter )
    {
        targetEntity = targetCharacter.GetComponent<EntityController>();
        characterStats = targetCharacter.GetComponent<CharacterStats>();

        UpdateHealth();
        SetId();
        characterStats.currentHealthUpdated += UpdateHealth;
    }

    void SetId()
    {
        id.text = $"{ characterStats.NetworkObjectId }";
    }

    void UpdateHealth()
    {
        health.text = $"{characterStats.currentHealth.Value}";
    }
}
