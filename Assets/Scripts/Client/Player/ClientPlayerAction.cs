using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class ClientPlayerActions : NetworkBehaviour
    {
        public void RequestAttack()
        {
            if ( !IsOwner )
                return;

            RequestAttackServerRpc();
        }

        [ServerRpc]
        private void RequestAttackServerRpc()
        {
            // NO lógica aquí
            Server.Player.ServerPlayerActions.HandleAttack( OwnerClientId );
        }
    }
}
