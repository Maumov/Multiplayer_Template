using Server.Combat;
using Unity.Netcode;

namespace Server.Player
{
    public static class ServerPlayerActions
    {
        public static void HandleAttack( ulong attackerId )
        {
            // Validaciones
            if ( !NetworkManager.Singleton.ConnectedClients.ContainsKey( attackerId ) )
                return;

            ServerCombatSystem.Instance.ProcessAttack( attackerId );
        }
    }
}
