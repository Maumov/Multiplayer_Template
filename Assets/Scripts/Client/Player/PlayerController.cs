using Shared.Combat;
using System.Collections.Generic;
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
        private TargetFinder targetFinder;

        public Transform spawnPoint; // opcional, referencia al CharacterSpawnPoint

        public delegate void PlayerControllerDelegate();
        public event PlayerControllerDelegate OnHealthChange;


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

            /*
            // Instancia local
            characterInstance = Instantiate( characterPrefab, spawnPoint.position, spawnPoint.rotation );
            characterInstance.transform.SetParent( spawnPoint, true );

            // Spawn en la red
            var netObj = characterInstance.GetComponent<NetworkObject>();
            if ( netObj != null )
            {
                // OwnerClientId = cliente dueño
                netObj.SpawnWithOwnership( OwnerClientId );
                targetFinder = characterInstance.GetComponent<TargetFinder>();
            }
            */

            /*
            // Si el personaje tiene cámara, activa solo para el owner
            var cam = characterInstance.GetComponentInChildren<Camera>();
            if ( cam )
                cam.enabled = IsOwner;
            */
            Vector3 spawnPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            SpawnCharacterServerRpc( spawnPosition );
        }
        [ServerRpc]
        void SpawnCharacterServerRpc( Vector3 spawnPosition )
        {
            if ( !IsServer )
                return; // Extra seguridad

            // Instanciamos el personaje
            characterInstance = Instantiate( characterPrefab, spawnPosition, Quaternion.identity );
            var netObj = characterInstance.GetComponent<NetworkObject>();

            // Damos ownership al jugador
            netObj.SpawnWithOwnership( OwnerClientId );

            // Guardamos el NetworkObjectId para referencia futura
            //CharacterNetId.Value = netObj.NetworkObjectId;
        }

        public void TakeDamage( int damage )
        {
            if ( !IsServer )
                return;
            health.Value -= damage;
            OnHealthChange?.Invoke();
            if ( health.Value <= 0 )
            {
                // Lógica muerte
            }
        }
        
        public void Move()
        {
            if ( characterInstance == null)
                return;

            characterInstance.transform.position = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
        }

        public void RequestAttack()
        {
            List<ulong> targets = targetFinder.GetTarget();
            ulong targetId = targets[ 0 ];
            RequestAttackServerRpc( targetId );
        }

        [ServerRpc]
        private void RequestAttackServerRpc( ulong target)
        {
            // NO lógica aquí
            Server.Player.ServerPlayerActions.HandleAttack( OwnerClientId, target );
        }
    }
}