using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class ClientPlayerActions : NetworkBehaviour
    {
        PlayerController controller;

        private void Start()
        {
            controller = GetComponent<PlayerController>();
        }
        public void RequestAttack()
        {
            if ( !IsOwner )
                return;

            controller.RequestAttack();
        }

        public void RequestMove()
        {
            Debug.Log( "Entro0" );
            if ( !IsOwner )
            {
                Debug.Log( "Entro1" );
                return;
            }
            Debug.Log("Entro2");

            controller.Move();
            //RequestMoveServerRpc();
        }
        
        /*
        [ServerRpc]
        private void RequestMoveServerRpc()
        {
            // NO lógica Esto funciona porque el PlayerPrefab tiene un NetworkTransform
            transform.position = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
        }
        */
    }
}
