using Server.Combat;
using Shared.Combat;
using System.Collections.Generic;
using Unity.Netcode;

namespace Server.Player
{
    public static class ServerPlayerActions
    {
        public static void HandleAttack( ulong attackerId, ulong target )
        {
            // Validaciones
            if ( !NetworkManager.Singleton.ConnectedClients.ContainsKey( attackerId ) )
                return;

            ServerCombatSystem.Instance.ProcessAttack( attackerId, target );
        }
    }
}
