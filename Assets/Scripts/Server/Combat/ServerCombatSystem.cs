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
            if ( !IsServer )
                return;

            // Ejemplo: buscar objetivo
            //IDamageable target = FindTarget( attackerId );
            /*
            if ( target.Count <= 0)
                return;

            IDamageable target = targets[ 0 ];
            if ( target == null )
                return;
            */



            if ( !NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue( target, out var targetNetObj ) )
                return;

            //target
            IDamageable damageable = targetNetObj.GetComponent<IDamageable>();

            //Damage data.
            DamageData data = new DamageData
            {
                BaseDamage = 25,
                CriticalChance = 0.0f
            };
            DamageResult result = DamageCalculator.Calculate( data );

            //Apply damage
            damageable.ApplyDamage( result );
        }

        private IDamageable FindTarget( ulong attackerId )
        {
            // Placeholder: raycast, range check, etc.
            return null;
        }
    }
}
