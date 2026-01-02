using Shared.Combat;
using UnityEngine;

namespace Server.Combat
{
    public static class DamageCalculator
    {
        public static DamageResult Calculate( DamageData data )
        {
            bool isCrit = Random.value <= data.CriticalChance;
            float finalDamage = isCrit ? data.BaseDamage * 2f : data.BaseDamage;

            return new DamageResult
            {
                FinalDamage = Mathf.RoundToInt( finalDamage ),
                IsCritical = isCrit
            };
        }
    }
}
