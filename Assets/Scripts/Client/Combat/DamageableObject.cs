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
        Debug.Log( $"Apply Damage in IDamageable" );
        controller.TakeDamage( result.FinalDamage );
    }
}
