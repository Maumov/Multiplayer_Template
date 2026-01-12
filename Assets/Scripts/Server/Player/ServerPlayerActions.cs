using Server.Combat;
using Unity.Netcode;
using UnityEngine;

namespace Server.Player
{
    public static class ServerPlayerActions
    {
        public static void HandleAttack( ulong attackerId, ulong target )
        {
            // Validaciones
            if ( !NetworkManager.Singleton.ConnectedClients.ContainsKey( attackerId ) )
                return;

            if (attackerId == target)
            {
                Debug.Log( "Attack was to self" );
                return;
            }
            Debug.Log( $"Processing attack from {attackerId} to {target}."  );
            ServerCombatSystem.Instance.ProcessAttack( attackerId, target );
        }
    }
}
