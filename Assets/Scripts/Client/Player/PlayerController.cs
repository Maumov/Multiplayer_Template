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

        [Header( "UI" )]
        public GameObject UIPrefab;
        private PlayerUIController uiController;

        public NetworkVariable<ulong> CharacterNetId = new( 0 );
        public Transform spawnPoint; // opcional, referencia al CharacterSpawnPoint


        public delegate void PlayerControllerDelegate();
        public event PlayerControllerDelegate OnHealthChange;

        public override void OnNetworkSpawn()
        {
            if ( IsOwner )
            {
                DontDestroyOnLoad( gameObject ); // mantiene este player al cambiar de escena
            }

            // Se ejecuta en host y clientes
            CharacterNetId.OnValueChanged += OnCharacterAssigned;

            // Si ya hay Character (late joiner)
            if ( CharacterNetId.Value != 0 )
                OnCharacterAssigned( 0, CharacterNetId.Value );
        }

        void Start()
        {
            if ( !IsOwner )
                return; // solo el dueño instancia su personaje

            if ( spawnPoint == null )
                spawnPoint = transform; // fallback

        }

        #region SPAWN CHARACTER
        public void SpawnCharacter()
        {
            if ( !IsOwner )
                return;

            if ( characterPrefab == null )
                return;

            if ( characterInstance != null ) 
                return;

            Vector3 spawnPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            SpawnCharacterServerRpc( spawnPosition, OwnerClientId );
        }

        [ServerRpc]
        void SpawnCharacterServerRpc( Vector3 spawnPosition, ulong _ownerClientId )
        {
            if ( !IsServer )
                return; // Extra seguridad

            // Si ya tiene Character, no spawnea otro
            if ( CharacterNetId.Value != 0 )
                return;

            // Instanciamos el personaje
            GameObject go = Instantiate( characterPrefab, spawnPosition, Quaternion.identity );
            NetworkObject netObj = go.GetComponent<NetworkObject>();

            // Spawn con ownership del cliente
            netObj.SpawnWithOwnership( _ownerClientId );
            // Guardamos NetworkObjectId para que los clientes lo resuelvan
            CharacterNetId.Value = netObj.NetworkObjectId;
        }

        private void OnCharacterAssigned( ulong oldId, ulong newId )
        {
            if ( NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue( newId, out var netObj ) )
            {
                characterInstance = netObj.gameObject;
                characterInstance.GetComponent<EntityController>().Init( this );
                if ( IsOwner )
                {
                    //Setup local stuff...
                    targetFinder = characterInstance.GetComponent<TargetFinder>();
                    //setup UI
                    GameObject ui = Instantiate( UIPrefab );
                    uiController = ui.GetComponent<PlayerUIController>();
                    uiController.Init( this );
                }
            }
        }
        #endregion

        #region RECEIVE DAMAGE
        public void TakeDamage( int damage )
        {
            Debug.Log( $"Start" );
            Debug.Log( $"Take Damage in PlayerController" );
            health.Value -= damage;
            OnHealthChange?.Invoke();
            if ( health.Value <= 0 )
            {
                // Lógica muerte
            }
            Debug.Log( $"Take Damage End" );
        }
        #endregion

        #region ATTACK
        public void RequestAttack()
        {
            List<ulong> targets = targetFinder.GetTarget();
            if ( targets.Count == 0 )
                return;
            
            ulong targetId = targets[ 0 ];
            RequestAttackServerRpc( targetId );
        }

        [ServerRpc]
        private void RequestAttackServerRpc( ulong target )
        {
            // NO lógica aquí
            Server.Player.ServerPlayerActions.HandleAttack( OwnerClientId, target );
        }
        #endregion

        #region MOVE
        public void Move()
        {
            Debug.Log( "Entro3" );
            if ( !IsOwner || characterInstance == null )
            {
                Debug.Log( $"Entro4 { canControlCharacter}, {characterInstance}" );
                return;
            }
            Debug.Log( $"Entro5 {canControlCharacter}, {characterInstance}" );

            Vector3 newPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            RequestMoveServerRPC( newPosition );
        }

        [ServerRpc]
        public void RequestMoveServerRPC( Vector3 position )
        {
            if ( !IsServer )
                return; // Extra seguridad

            characterInstance.transform.position = position;
        }
        #endregion

    }
}