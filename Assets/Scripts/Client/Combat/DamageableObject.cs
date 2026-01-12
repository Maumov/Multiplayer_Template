using Client.Player;
using Shared.Combat;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    EntityController controller;

    void Start()
    {
        controller = GetComponent<EntityController>();
    }
    public void ApplyDamage( DamageResult result )
    {
        controller.TakeDamage( result.FinalDamage );
    }
}
