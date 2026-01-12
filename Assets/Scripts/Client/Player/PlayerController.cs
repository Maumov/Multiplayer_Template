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
        private bool canControlCharacter;
        private TargetFinder targetFinder;

        public NetworkVariable<ulong> CharacterNetId = new( 0 );

        public Transform spawnPoint; // opcional, referencia al CharacterSpawnPoint


        public delegate void PlayerControllerDelegate();
        public event PlayerControllerDelegate OnHealthChange;

        /*
        public override void OnNetworkSpawn()
        {
            // Esto se ejecuta en clientes y host
            CharacterNetId.OnValueChanged += OnCharacterAssigned;

            // Si ya hay un valor (late joiner), asignamos referencia
            if ( CharacterNetId.Value != 0 )
                OnCharacterAssigned( 0, CharacterNetId.Value );
        }
        private void OnCharacterAssigned( ulong oldId, ulong newId )
        {
            if ( NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue( newId, out var netObj ) )
            {
                characterInstance = netObj.gameObject;
                // Checamos si ahora somos owner del Character
                if ( netObj.IsOwner )
                {
                    // Activar control del personaje
                    canControlCharacter = true;
                }
                else
                {
                    // No somos owner → no movemos
                    canControlCharacter = false;
                }
            }
        }
        */

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

            Vector3 spawnPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            SpawnCharacterServerRpc( spawnPosition );
        }

        [ServerRpc]
        void SpawnCharacterServerRpc( Vector3 spawnPosition )
        {
            if ( !IsServer )
                return; // Extra seguridad

            // Si ya tiene Character, no spawnea otro
            if ( CharacterNetId.Value != 0 )
                return;

            // Instanciamos el personaje
            GameObject go = Instantiate( characterPrefab, spawnPosition, Quaternion.identity );
            NetworkObject netObj = go.GetComponent<NetworkObject>();

            // Damos ownership al jugador
            netObj.SpawnWithOwnership( OwnerClientId );
            characterInstance = go;
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
            Debug.Log( "Entro3" );
            if ( !canControlCharacter || characterInstance == null )
            {
                Debug.Log( $"Entro4 { canControlCharacter}, {characterInstance}" );
                return;
            }
            Debug.Log( "Entro5" );

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