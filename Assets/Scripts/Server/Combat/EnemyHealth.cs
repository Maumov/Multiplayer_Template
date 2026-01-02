using Shared.Combat;
using Unity.Netcode;
using UnityEngine;

namespace Server.Combat
{
    public class EnemyHealth : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        public override void OnNetworkSpawn()
        {
            if ( !IsServer )
                return;
            currentHealth = maxHealth;
        }

        public void ApplyDamage( DamageResult result )
        {
            if ( !IsServer )
                return;

            currentHealth -= result.FinalDamage;

            NotifyDamageClientRpc( result.FinalDamage, result.IsCritical );

            if ( currentHealth <= 0 )
                Die();
        }

        private void Die()
        {
            NetworkObject.Despawn();
        }

        [ClientRpc]
        private void NotifyDamageClientRpc( int damage, bool critical )
        {
            
            // 👇 SOLO FEEDBACK (sin lógica)
            
            Vector3 hitPosition = transform.position;

            Client.Combat.CombatEvents
                .RaiseEnemyDamaged( hitPosition, damage, critical );
        }
    }
}
