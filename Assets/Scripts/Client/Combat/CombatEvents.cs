using System;
using UnityEngine;

namespace Client.Combat
{
    public static class CombatEvents
    {
        public static event Action<Vector3, int, bool> OnEnemyDamaged;

        public static void RaiseEnemyDamaged(
            Vector3 position,
            int damage,
            bool critical )
        {
            OnEnemyDamaged?.Invoke( position, damage, critical );
            Debug.Log( $"Damage: {damage} CRIT: {critical}" );
        }
    }
}
