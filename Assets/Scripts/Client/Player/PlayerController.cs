using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header( "Player Stats" )]
        public NetworkVariable<int> health = new NetworkVariable<int>( 100 );

        [Header( "Character" )]
        public GameObject characterPrefab; // prefab visual / jugable
        private GameObject characterInstance;

        public Transform spawnPoint; // opcional, referencia al CharacterSpawnPoint

        void Start()
        {
            if ( !IsOwner )
                return; // solo el dueño instancia su personaje

            if ( spawnPoint == null )
                spawnPoint = transform; // fallback

        }

        public void SpawnCharacter()
        {
            if ( !IsOwner )
                return;

            if ( characterPrefab == null )
                return;

            if ( characterInstance != null ) 
                return;

            // Instancia local
            //characterInstance = Instantiate( characterPrefab, spawnPoint.position, spawnPoint.rotation );
            //characterInstance.transform.SetParent( spawnPoint, true );

            // Spawn en la red
            var netObj = characterInstance.GetComponent<NetworkObject>();
            if ( netObj != null )
            {
                // OwnerClientId = cliente dueño
                netObj.SpawnWithOwnership( OwnerClientId );
            }


            // Si el personaje tiene cámara, activa solo para el owner
            var cam = characterInstance.GetComponentInChildren<Camera>();
            if ( cam )
                cam.enabled = IsOwner;
        }

        // Métodos de PlayerPrefab que no necesitan ser visibles
        [ServerRpc]
        public void TakeDamageServerRpc( int damage )
        {
            if ( !IsServer )
                return;
            health.Value -= damage;
            if ( health.Value <= 0 )
            {
                // Lógica muerte
            }
        }
    }
}