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

        public void RequestMove()
        {
            if ( !IsOwner )
                return;

            RequestMoveServerRpc();
        }

        [ServerRpc]
        private void RequestMoveServerRpc()
        {
            // NO lógica aquí
            transform.position = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
        }
        
    }
}
