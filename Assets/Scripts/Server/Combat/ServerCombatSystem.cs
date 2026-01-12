using Shared.Combat;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Server.Combat
{
    public class ServerCombatSystem : NetworkBehaviour
    {
        public static ServerCombatSystem Instance;

        void Awake()
        {
            Instance = this;
        }

        public void ProcessAttack( ulong attackerId, ulong target )
        {
            Debug.Log( $"Start ProcessAttack" );
            if ( !IsServer )
                return;

            Debug.Log( $"Finding Id" );
            if ( !NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue( target, out var targetNetObj ) )
            {
                Debug.Log( $"Id not found" );
                return;
            }

            Debug.Log( $"Damage processing Start" );
            //target
            IDamageable damageable = targetNetObj.GetComponent<IDamageable>();

            //Damage data.
            DamageData data = new DamageData
            {
                BaseDamage = 25,
                CriticalChance = 0.0f
            };
            DamageResult result = DamageCalculator.Calculate( data );
            Debug.Log( $"Damage processing End" );
            //Apply damage
            Debug.Log( $"Applying damage" );
            damageable.ApplyDamage( result );
        }

        private IDamageable FindTarget( ulong attackerId )
        {
            // Placeholder: raycast, range check, etc.
            return null;
        }
    }
}
