using Client.Player;
using Shared.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    PlayerController playerController;

    public void ApplyDamage( DamageResult result )
    {
        playerController.TakeDamage( result.FinalDamage );
    }
}
