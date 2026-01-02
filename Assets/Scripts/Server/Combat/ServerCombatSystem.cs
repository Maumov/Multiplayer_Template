using Shared.Combat;
using UnityEngine;

namespace Server.Combat
{
    public class ServerCombatSystem : MonoBehaviour
    {
        public static ServerCombatSystem Instance;

        void Awake()
        {
            Instance = this;
        }

        public void ProcessAttack( ulong attackerId )
        {
            // Ejemplo: buscar objetivo
            IDamageable target = FindTarget( attackerId );
            if ( target == null )
                return;

            DamageData data = new DamageData
            {
                BaseDamage = 25,
                CriticalChance = 0.2f
            };

            DamageResult result = DamageCalculator.Calculate( data );
            target.ApplyDamage( result );
        }

        private IDamageable FindTarget( ulong attackerId )
        {
            // Placeholder: raycast, range check, etc.
            return null;
        }
    }
}
